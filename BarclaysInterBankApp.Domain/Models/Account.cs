using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace BarclaysInterBankApp.Domain.Models
{
    public class Account
    {     
            public Guid Id { get; set; }
            public string AccountNumber { get; set; }
            public string PinHash { get; set; }
            public string AccountName { get; set; }
            public string Email { get; set; }
            public string PhoneNumber { get; set; }
            public AccountType AccountType { get; set; }
            public decimal CurrentAccountBalance { get; set; }
            public DateTime DateCreated { get; set; }
            public DateTime DateUpdated { get; set; }      
    }
    public enum AccountType
    {
        SAVINGS,
        CURRENT,
    }
}

