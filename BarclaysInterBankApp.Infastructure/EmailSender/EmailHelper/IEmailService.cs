using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarclaysInterBankApp.Infastructure.EmailSender.EmailHelper
{
    public interface IEmailService
    {
        void SendEmail(string from,string to,string subject,string message);
    }
}
