using BarclaysInterBankApp.Application.AccountManager;
using BarclaysInterBankApp.Application.Contract;
using BarclaysInterBankApp.Application.Response;
using BarclaysInterBankApp.Application.Response.InitiateTransfer;
using BarclaysInterBankApp.Application.Utility;
using BarclaysInterBankApp.Domain.Models;
using BarclaysInterBankApp.Domain.Models.Paystack;
using BarclaysInterBankApp.Infastructure.DataAccess;
using BarclaysInterBankApp.Infastructure.EmailSender.EmailHelper;
using BarclaysInterBankApp.Infastructure.EmailUtility;
using BarclaysInterBankApp.Infastructure.ExceptionHandler;
using BarclaysInterBankApp.Infastructure.HttpHelper;
using BarclaysInterBankApp.Infastructure.HttpHelper.Contract;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace BarclaysInterBankApp.Application.Implementation
{
    public class TransferRepository : ITransferRepository
    {
        private readonly ILogger<TransferRepository> _logger;
        private readonly IHttpService _httpService;
        private readonly PaystackSettings _paystackSettings;
        private readonly ApplicationDbContext _context;
        private readonly IEmailService _emailService;
        private readonly EmailConfiguration _emailConfiguration;
        private string type = "nuban";
        private string Currency = "NGN";
        private string sourceFrom = "balance";

        public TransferRepository(ILogger<TransferRepository> logger, IHttpService httpService, IOptions<PaystackSettings> paystackSettings,ApplicationDbContext context,IOptions<EmailConfiguration> emailConfiguration,IEmailService emailService)
        {
            _logger = logger;
            _httpService = httpService;
            _paystackSettings = paystackSettings.Value;
            _context = context;
            _emailService = emailService;
            _emailConfiguration = emailConfiguration.Value;

        }
        public async Task<TransferResponse> MakeTransfer(PaystackTransferModel paystackTransfer, string PinHash)
        {
            try
            {
                var accountNumber = _context.Accounts.SingleOrDefault(A => A.AccountNumber == paystackTransfer.Source_Bank_Account);
                if (accountNumber == null)
                {
                    throw new Exception("Account not found.");
                }

                if (string.IsNullOrEmpty(PinHash) || accountNumber.PinHash != AccountGenerateManager.HashPin(PinHash) || paystackTransfer.Amount < 0)
                {
                    throw new Exception("Invalid Pin or amount.");
                }

                string beneficiaryName = string.Empty;
                ApiResponse<AccountResolveResponse> verify = await VerifyBeneficiaryAccount(paystackTransfer.Destination_Bank_Code, paystackTransfer.Destination_Account);
                if (verify != null && verify.IsSuccess && verify.Data?.status == true)
                {
                    beneficiaryName = verify.Data.data.account_name;
                }
                else
                {
                    throw new Exception("Beneficiary Account could not be verified at this point or is invalid.");
                }

                string recepientCode = string.Empty;
                ApiResponse<CreateRecepientResponse> response = await CreateRecepient(type, beneficiaryName, paystackTransfer.Destination_Account, paystackTransfer.Destination_Bank_Code, Currency);
                if (response != null && response.IsSuccess && response.Data?.status == true)
                {
                    recepientCode = response.Data.data.recipient_code;             
                }
                else
                {
                    throw new Exception("Recipient cannot be generated at this point. Transfer failed.");
                }

                ApiResponse<InitiateTransfer> transferResponse = await InitiateTransfer(paystackTransfer.Reason, paystackTransfer.Destination_Account, paystackTransfer.Amount, recepientCode);
                if (transferResponse != null && transferResponse.IsSuccess && transferResponse.Data?.status == true)
                {
                    accountNumber.CurrentAccountBalance -= paystackTransfer.Amount;
                    var transaction = new Transaction
                    {
                        TransactionId = Guid.NewGuid(),
                        TransactionStatus = Domain.Enums.TransStatus.PENDING,
                        AccountNumber = accountNumber.AccountNumber,
                        IsSuccessful = true,
                        Amount = paystackTransfer.Amount,
                        TransactionDescription = $"PAYSTACK TRF IFO {beneficiaryName} WITH ACCT:{paystackTransfer.Destination_Account}",
                        TransactionReference = $"{transferResponse.Data.data.reference}",
                        Type = Domain.Enums.TransactionType.TRANSFER,
                        BeneficiaryName = beneficiaryName,
                        BeneficiaryNumber = paystackTransfer.Destination_Account,
                        Timestamp = DateTime.Now,
                        AccountId = accountNumber.Id,
                        CurrentBalance = accountNumber.CurrentAccountBalance
                    };
                    await _context.Transactions.AddAsync(transaction);
                    await _context.SaveChangesAsync();

                    string subject = "TRANSACTION NOTIFICATION";
                    string message = AccountGenerateManager.GetTransactionMessage(transaction);
                    string From = _emailConfiguration.FromEmail;
                    _emailService.SendEmail(From, accountNumber.Email, subject, message);

                    // Log transfer information
                    Log.Information("Transfer successful: {@Transaction}", transaction);

                    return new TransferResponse
                    {
                        Status = true,
                        Message = $"NGN{transferResponse.Data.data.amount}{transferResponse.Data.message} with reference number: {transferResponse.Data.data.reference}"
                    };
                }
                else
                {
                    throw new Exception("Transfer failed.");
                }
            }
            catch (Exception ex)
            {
                // Log error
                Log.Error(ex, "Transfer failed: {Message}", ex.Message);
                throw new TransferFailureException("Transfer failed.");
            }
        }

        private async Task<ApiResponse<AccountResolveResponse>> VerifyBeneficiaryAccount(string bankCode, string accountNumber)
        {
            Uri accountResolveUrl = _paystackSettings.AccountResolveUrl;
            string urlWithParams = $"{accountResolveUrl}account_number={accountNumber}&bank_code={bankCode}";
            Uri requestUri = new Uri(urlWithParams);
            string authToken = _paystackSettings.Secretkey;
            ApiResponse<AccountResolveResponse> verify = await _httpService.MakeHttpRequestAsync<AccountResolveResponse>(requestUri, null, authToken, AuthType.Bearer, CustomHttpMethod.GET);
            if (verify.IsSuccess && verify.Data?.status == true)
            {
                return verify;
            }
            else
            {
                throw new BeneficiaryVerificationException("Beneficiary Account could not be verified at this point or is invalid.");
            }
        }

        private async Task<ApiResponse<CreateRecepientResponse>> CreateRecepient(string type, string name, string accountNumber, string bankCode, string currency)
        {
            Uri CreateAccountUrl = _paystackSettings.CreateAccountUrl;
            string authToken = _paystackSettings.Secretkey;
            //add payload that will be serialized
            var payload = new CreateRecepient { type = type, name = name, currency = currency, account_number = accountNumber, bank_code = bankCode };
            var jsonPayLoad = JsonConvert.SerializeObject(payload);
            ApiResponse<CreateRecepientResponse> response = await _httpService.MakeHttpRequestAsync<CreateRecepientResponse>(CreateAccountUrl, jsonPayLoad, authToken, AuthType.Bearer, CustomHttpMethod.POST);
            if (response.IsSuccess && response.Data?.status == true)
            {
                return response;
            }
            else
            {
                throw new RecepientCreationException("Recipient cannot be generated at this point. Transfer failed.");
            }
        }

        private string GeneratePaystackTransferReference()
        {
            string today = DateTime.Now.ToString("ddMMyyyyHHmmss");
            int random = new Random().Next(10000, 99999);
            return today + random.ToString();
        }

        private async Task<ApiResponse<InitiateTransfer>> InitiateTransfer(string reason, string beneficiaryAccount, decimal amount, string recipientCode)
        {
            string reference = AccountGenerateManager.GenerateReferenceNumber();
            Uri transferUrl = _paystackSettings.transferUrl;
            string authToken = _paystackSettings.Secretkey;
            //add payload that will be serialized
            var payload = new InitiateTransferPayload { source = sourceFrom, reason = reason, amount = amount, reference = reference, recipient = recipientCode };
            var jsonPayLoad = JsonConvert.SerializeObject(payload);
            ApiResponse<InitiateTransfer> response = await _httpService.MakeHttpRequestAsync<InitiateTransfer>(transferUrl, jsonPayLoad, authToken, AuthType.Bearer, CustomHttpMethod.POST);
            if (response.IsSuccess && response.Data?.status == true)
            {
                return response;
            }
            else
            {
                throw new TransferFailureException("Transfer failed.");
            }
        }

    }
}
