using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarclaysInterBankApp.Infastructure.ExceptionHandler
{
    public class BeneficiaryVerificationException : Exception
    {
        public BeneficiaryVerificationException(string message) : base(message)
        {
            
        }
    }

    public class RecepientCreationException : Exception
    {
        public RecepientCreationException(string message) : base(message)
        {

        }
    }

    public class TransferFailureException : Exception
    {
        public TransferFailureException(string message) : base(message)
        {

        }
    }

    public class  EmailNotSentException : Exception
    {
        public EmailNotSentException(string message) : base(message) 
        {
            
        }
    }

    public class AccountNotFountException : Exception
    {
        public AccountNotFountException(string message) : base(message) 
        {
            
        }
    }
}
