using BarclaysInterBankApp.Application.Request;
using BarclaysInterBankApp.Domain.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace BarclaysInterBankApp.Application.AccountManager
{
    public class AccountGenerateManager
    {
        public static string GenerateAccountNumber(AccountCreateRequest account)
        {
            Random random = new Random();
            int Sum = 0;
            int[] digits = new int[9];

            for(var i=0; i< digits.Length; i++)
            {
                digits[i] = random.Next(0, 10);
                Sum = Sum + digits[i];
            }
            digits[8] = 70 - Sum;
            digits[8] = digits[8] % 10;
            string accountNumber;
            if (account.AccountType == AccountType.CURRENT )
            {
                accountNumber  = "1" + string.Join( "", digits);
            }
            else if(account.AccountType == AccountType.SAVINGS)
            {
                accountNumber = "2" + string.Join("", digits);
            }
            else
            {
                accountNumber = null;
            }
            return accountNumber;
        }


        public static string HashPin(string pin)
        {
            using (var sha256 = SHA256.Create()) 
            { 
                var pinBytes = Encoding.UTF8.GetBytes(pin);
                var hashBytes = sha256.ComputeHash(pinBytes);
                var hash = BitConverter.ToString(hashBytes).Replace("_", string.Empty);
                return hash;
            }
        }

        public static string GenerateReferenceNumber()
        {
            string today = DateTime.Now.ToString("ddMMyyyyHHmmss");
            int random = new Random().Next(10000, 99999);
            return today + random.ToString();
        }


        public static string GetTransactionMessage(Transaction transaction)
        {
            //string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory + "EmailTemplate.html");
            //string emailbody = File.ReadAllText(filePath);
            Assembly assembly = Assembly.GetExecutingAssembly();
            string emailTemplate = "BarclaysInterBankApp.Application.Transaction.html";
            try
            {
                using Stream stream = assembly.GetManifestResourceStream(emailTemplate);
                using StreamReader reader = new(stream);
                string emailbody = reader.ReadToEnd();
                emailbody = emailbody.Replace("[dynamic account number]", MaskAccountNumber(transaction.AccountNumber))
                             .Replace("[dynamic date of transaction]", DateTime.Now.ToString())
                             .Replace("[dynamic value date]", DateTime.Now.ToString())
                             .Replace("[dynamic currency]", "NGN")
                             .Replace("[dynamic description]", transaction.TransactionDescription)
                             .Replace("[dynamic reference code]", transaction.TransactionReference)
                             .Replace("[dynamic transaction type]", transaction.Type.ToString())
                             .Replace("[dynamic amount]", transaction.Amount.ToString())
                            .Replace("[dynamic current balance]", transaction.CurrentBalance.ToString());
                return emailbody;
            }
            catch (Exception)
            {
                throw new Exception("Failed to read email template from memory");
            }
        }


        public static string GetAccountOpeningMessage(Account account)
        {
            //string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory + "AccountOpening.html");
            //string emailbody = File.ReadAllText(filePath);
            Assembly assembly = Assembly.GetExecutingAssembly();
            string[] resourceNames = assembly.GetManifestResourceNames();
            string resourcePath = null;
            string targetNamespace = "BarclaysInterBankApp.Application.AccountOpening.html";

            foreach (string resourceName in resourceNames)
            {
               // Console.WriteLine(resourceName);
                if (resourceName.Contains(targetNamespace))
                {
                    resourcePath = resourceName;
                    break;
                }
            }

            if (resourcePath != null)
            {
                try
                {
                    using Stream stream = assembly.GetManifestResourceStream(resourcePath);
                    using StreamReader reader = new(stream);
                    string emailbody = reader.ReadToEnd();
                    emailbody = emailbody.Replace("[dynamic account number]", account.AccountNumber)
                                 .Replace("[dynamic account name]", account.AccountName);
                    return emailbody;
                }
                catch (Exception)
                {
                    throw new Exception("Failed to read email template from memory");
                }
            }
            else
            {     
              throw new Exception("File not Found");
            }
        }



        private static string MaskAccountNumber(string accountNumber)
        {
            if (string.IsNullOrEmpty(accountNumber))
            {
                return string.Empty;
            }
            int digitToKeep = 3;
            string firstThreeDigits = accountNumber.Substring(0, digitToKeep);
            string lastThreeDigits = accountNumber.Substring(accountNumber.Length - digitToKeep);
            int digitToMask = accountNumber.Length - (2 * digitToKeep);
            string mask = new string('*', digitToMask);
            string maskedAccountNumber = firstThreeDigits + mask + lastThreeDigits;
            return maskedAccountNumber;
        }


    }
}
