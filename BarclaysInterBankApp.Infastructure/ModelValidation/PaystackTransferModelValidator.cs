using BarclaysInterBankApp.Domain.Models.Paystack;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarclaysInterBankApp.Infastructure.ModelValidation
{
    public class PaystackTransferModelValidator : AbstractValidator<PaystackTransferModel>
    {
        public PaystackTransferModelValidator() 
        {
            RuleFor(paystack => paystack.Source_Bank_Account).NotEmpty().Length(10);
            RuleFor(paystack => paystack.Destination_Bank_Code).NotEmpty().Length(03);
            RuleFor(paystack => paystack.Destination_Account).NotEmpty().Length(10);
            RuleFor(paystack => paystack.Amount).NotEmpty().GreaterThan(0);
        }
    }
}
