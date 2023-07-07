using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarclaysInterBankApp.Application.Request
{
    public class TopUpRequest
    {
        public string AccountNumber { get; set; }
        public string PinHash { get; set; }
    }
}
