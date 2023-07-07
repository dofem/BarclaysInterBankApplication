using BarclaysInterBankApp.Infastructure.HttpHelper;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarclaysInterBankApp.Application.Response
{
    public class AccountCreateResponse 
    {
        public bool IsSuccess { get; set; }
        public string Status { get; set; }
        public string Message { get; set; }
    }
}
