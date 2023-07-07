using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarclaysInterBankApp.Application.Utility
{
    public class PaystackSettings
    {
        public Uri AccountResolveUrl { get; set; }
        public string Secretkey { get; set; }
        public Uri CreateAccountUrl { get; set; }
        public Uri transferUrl { get; set; }
    };
}
