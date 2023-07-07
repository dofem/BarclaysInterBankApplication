using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarclaysInterBankApp.Domain.Models.Paystack
{

    public class InitiateTransferPayload
    {
        public string source { get; set; }
        public decimal amount { get; set; }
        public string reference { get; set; }
        public string recipient { get; set; }
        public string reason { get; set; }
    }

}
