using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using BarclaysInterBankApp.Domain.Enums;

namespace BarclaysInterBankApp.Domain.Models
{
    public class Transaction
    {
        public Guid TransactionId { get; set; }
        public string TransactionReference { get; set; }
        public string AccountNumber { get; set; }
        public string BeneficiaryNumber { get; set; }
        public string BeneficiaryName { get; set; }
        public bool IsSuccessful { get; set; }
        public TransStatus TransactionStatus { get; set; }
        public decimal Amount { get; set; }
        public TransactionType Type { get; set; }
        public string TransactionDescription { get; set; }
        public DateTime Timestamp { get; set; }
        public Guid AccountId { get; set; }
        public decimal CurrentBalance { get; set; }
        public Account Account { get; set; }
    }
}
