using BarclaysInterBankApp.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarclaysInterBankApp.Application.Request
{
    public class AccountCreateRequest
    {
        public string AccountName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public AccountType AccountType { get; set; }
    }
}
