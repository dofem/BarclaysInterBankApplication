using AutoMapper;
using BarclaysInterBankApp.Application.AccountManager;
using BarclaysInterBankApp.Application.Contract;
using BarclaysInterBankApp.Application.Request;
using BarclaysInterBankApp.Application.Response;
using BarclaysInterBankApp.Domain.Models;
using BarclaysInterBankApp.Infastructure.DataAccess;
using BarclaysInterBankApp.Infastructure.EmailSender.EmailHelper;
using BarclaysInterBankApp.Infastructure.EmailUtility;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarclaysInterBankApp.Application.Implementation
{
    public class AccountRepository : IAccountRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailService _emailService;
        private readonly IMapper _mapper;
        private readonly EmailConfiguration _emailConfiguration;

        public AccountRepository(ApplicationDbContext context, IEmailService emailService, IMapper mapper,IOptions<EmailConfiguration> emailConfiguration)
        {
            _context = context;
            _emailService = emailService;
            _mapper = mapper;
            _emailConfiguration = emailConfiguration.Value;
        }
        public async Task<AccountCreateResponse> CreateAccount(AccountCreateRequest accountRequest)
        {
            try
            {
                var defaultPin = "0000";
                var accountDto = _mapper.Map<Account>(accountRequest);
                var account = new Account
                {
                    AccountName = accountDto.AccountName,
                    AccountType = accountDto.AccountType,
                    PhoneNumber = accountDto.PhoneNumber,
                    Email = accountDto.Email,
                    AccountNumber = AccountGenerateManager.GenerateAccountNumber(accountRequest),
                    DateCreated = DateTime.Now,
                    CurrentAccountBalance = 0,
                    PinHash = AccountGenerateManager.HashPin(defaultPin),
                    Id = new Guid()
                };
                _context.Accounts.Add(account);
                _context.SaveChanges();

                AccountCreateResponse response = new()
                {
                    IsSuccess = true,
                    Status = "Success",
                    Message = "Account Created Successfully"
                };
                //_emailService.SendEmail => Kindly send email here
                string subject = "ACCOUNT OPENING NOTIFICATION";
                string message = AccountGenerateManager.GetAccountOpeningMessage(account);
                string From = _emailConfiguration.FromEmail;
                _emailService.SendEmail(From,accountDto.Email,subject, message);
                return response;

            }
            catch (Exception)
            {
                AccountCreateResponse response = new()
                {
                    IsSuccess = false,
                    Status = "Failure",
                    Message = "Something went wrong,Account creation failed"
                };
                return response;


            }
        }

        public async Task<PinChangeResponse> ChangePin(ChangePinRequest changePinRequest, string newPin)
        {
            var account = _context.Accounts.Where(a => a.AccountNumber == changePinRequest.AccountNumber).FirstOrDefault();
            if (account == null)
            {
                return new PinChangeResponse
                {
                    IsSuccess = false,
                    Status = "Failure",
                    Message = "Account not found"
                };
            }
            var currentPinHash = AccountGenerateManager.HashPin(changePinRequest.PinHash);
            if (account.PinHash != currentPinHash)
            {
                return new PinChangeResponse
                {
                    IsSuccess = false,
                    Status = "Failure",
                    Message = "Invalid current PIN"
                };
            }
            // Perform additional checks if needed before changing the PIN
            var newPinHash = AccountGenerateManager.HashPin(newPin);
            account.PinHash = newPinHash;
            _context.Accounts.Update(account);
            _context.SaveChanges();

            return new PinChangeResponse
            {
                IsSuccess = true,
                Status = "Success",
                Message = "PIN successfully changed"
            };
        }
    }

}   

