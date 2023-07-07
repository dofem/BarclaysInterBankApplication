using BarclaysInterBankApp.Domain.Models;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarclaysInterBankApp.Infastructure.Model_Validation
{
    public class AccountValidator : AbstractValidator<Account>
    {
        public AccountValidator()
        {
            RuleFor(account => account.AccountNumber).NotEmpty().Length(10);
            RuleFor(account => account.PinHash).NotEmpty().Length(4);
            RuleFor(account => account.AccountName).NotEmpty().MaximumLength(100);
            RuleFor(account => account.Email).NotEmpty().EmailAddress();
            RuleFor(account => account.PhoneNumber).NotEmpty().Must(phoneNumber =>
            {
                if (phoneNumber.StartsWith("080") || phoneNumber.StartsWith("081") || phoneNumber.StartsWith("070"))
                {
                    return phoneNumber.Length == 11;
                }
                else if (phoneNumber.StartsWith("234"))
                {
                    return phoneNumber.Length == 13;
                }
                else
                {
                    return false;
                }
            }).WithMessage("Invalid phone number format.");
            RuleFor(account => account.AccountType).NotNull().IsInEnum();
            RuleFor(account => account.CurrentAccountBalance).GreaterThanOrEqualTo(0);
        }
    }
}
