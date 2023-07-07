using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarclaysInterBankApp.Domain.Models.Paystack
{
    public class PaystackTransferModel
    {
        public decimal Amount { get; set; }
        public string Destination_Bank_Code { get; set; }
        public string Reason { get; set; }
        public string Destination_Account { get; set; }
        public string Source_Bank_Account { get;set; }
    }
}
