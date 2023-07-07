using AutoMapper;
using BarclaysInterBankApp.Application.AccountManager;
using BarclaysInterBankApp.Application.Contract;
using BarclaysInterBankApp.Application.Request;
using BarclaysInterBankApp.Application.Response;
using BarclaysInterBankApp.Domain.Models;
using BarclaysInterBankApp.Infastructure.DataAccess;
using BarclaysInterBankApp.Infastructure.EmailSender.EmailHelper;
using BarclaysInterBankApp.Infastructure.EmailUtility;
using BarclaysInterBankApp.Infastructure.ExceptionHandler;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace BarclaysInterBankApp.Application.Implementation
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly EmailConfiguration _emailConfiguration;
        private readonly IEmailService _emailService;

        public TransactionRepository(ApplicationDbContext context,IMapper mapper,IOptions<EmailConfiguration> emailConfiguration,IEmailService emailService)
        {
            _context = context;
            _mapper = mapper;
            _emailConfiguration = emailConfiguration.Value;
            _emailService = emailService;
        }
        public async Task<TopUpResponse> PerformTopUp(TopUpRequest topUp, decimal amount)
        {
            try
            {
                var accountNumber = _context.Accounts.Where(A => A.AccountNumber == topUp.AccountNumber).FirstOrDefault();
                if (accountNumber == null)
                {
                    throw new AccountNotFountException("This Account does not exist in our database");
                }

                if (accountNumber.PinHash == AccountGenerateManager.HashPin(topUp.PinHash) && amount >= 0)
                {
                    var topUpDetails = _mapper.Map<Account>(topUp);
                    accountNumber.CurrentAccountBalance += amount;
                    var transaction = new Transaction
                    {
                        TransactionId = Guid.NewGuid(),
                        TransactionStatus = Domain.Enums.TransStatus.SUCCESSFUL,
                        AccountNumber = topUpDetails.AccountNumber,
                        IsSuccessful = true,
                        Amount = amount,
                        TransactionDescription = "BALANCE TOPUP IFO SELF",
                        TransactionReference = AccountGenerateManager.GenerateReferenceNumber(),
                        Type = Domain.Enums.TransactionType.TOPUP,
                        Timestamp = DateTime.UtcNow,
                        AccountId = accountNumber.Id,
                       CurrentBalance = accountNumber.CurrentAccountBalance
                    };
                    await _context.Transactions.AddAsync(transaction);
                    await _context.SaveChangesAsync();

                    string subject = "TRANSACTION NOTIFICATION";
                    string message = AccountGenerateManager.GetTransactionMessage(transaction);
                    string From = _emailConfiguration.FromEmail;
                    _emailService.SendEmail(From, accountNumber.Email, subject, message);

                    return new TopUpResponse
                    {
                        IsSuccess = true,
                        Status = "Successful",
                        Message = $"Transaction TopUp Successful, your transaction reference number is {transaction.TransactionReference}"
                    };                 

                }
                else
                {
                    TopUpResponse errorResponse = new TopUpResponse();
                    if (amount < 0)
                    {
                        errorResponse.Status = "Failure";
                        errorResponse.Message = $"{amount} is an invalid amount, Kindly insert a positive amount";
                    }
                    else
                    {
                        errorResponse.Status = "Failure";
                        errorResponse.Message = "Pin Incorrect, try again loser!!!";
                    }
                }
            }
            catch (Exception ex)
            {

                throw new Exception("Something went wrong!!!!");
            }
            return new TopUpResponse
            {
                IsSuccess = false,
                Status = "Failure",
                Message = "Something went wrong, Transaction could not be completed"
            };
            
        }
    }
}
