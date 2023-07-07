using BarclaysInterBankApp.Domain.Models.Paystack;
using BarclaysInterBankApp.Domain.Models;
using BarclaysInterBankApp.Infastructure.Model_Validation;
using BarclaysInterBankApp.Infastructure.ModelValidation;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation.AspNetCore;
using BarclaysInterBankApp.Application.Utility;
using Microsoft.Extensions.Configuration;
using BarclaysInterBankApp.Application.Contract;
using BarclaysInterBankApp.Application.Implementation;

namespace BarclaysInterBankApp.Application
{
    public static class ApplicationServiceExtension
    {
        public static void AddApplicationService(this IServiceCollection services)
        {     
            services.AddFluentValidationAutoValidation();
            services.AddTransient<IValidator<Account>, AccountValidator>();
            services.AddTransient<IValidator<PaystackTransferModel>, PaystackTransferModelValidator>();
            services.AddScoped<IAccountRepository, AccountRepository>();
            services.AddScoped<ITransactionRepository, TransactionRepository>();
            services.AddScoped<ITransferRepository, TransferRepository>();
        }
    }
}
