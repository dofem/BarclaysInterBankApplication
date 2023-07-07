using BarclaysInterBankApp.Infastructure.HttpHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace BarclaysInterBankApp.Application.Response
{
    public class TransferResponse
    {
        public bool Status { get; set; }
        public string Message { get; set; }
    }
}
