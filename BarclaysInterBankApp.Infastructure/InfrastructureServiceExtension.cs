using BarclaysInterBankApp.Infastructure.EmailSender.EmailHelper;
using BarclaysInterBankApp.Infastructure.HttpHelper.Contract;
using BarclaysInterBankApp.Infastructure.HttpHelper.Implementation;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarclaysInterBankApp.Infastructure
{
    public static class InfrastructureServiceExtension
    {
       public static void AddinfrastructureService(this IServiceCollection services)
       {
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IHttpService, HttpService>();
            services.AddHttpClient();
       }
    }
}
