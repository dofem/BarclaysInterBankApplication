using BarclaysInterBankApp.Infastructure.EmailUtility;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using BarclaysInterBankApp.Infastructure.ExceptionHandler;
using MimeKit;
using MailKit.Net.Smtp;
using MailKit.Security;

namespace BarclaysInterBankApp.Infastructure.EmailSender.EmailHelper
{
    public class EmailService : IEmailService
    {
        private readonly ILogger<EmailService> _logger;
        private readonly EmailConfiguration _emailConfiguration;

        public EmailService(ILogger<EmailService> logger,IOptions<EmailConfiguration> emailConfiguration)
        {
            _logger = logger;
            _emailConfiguration = emailConfiguration.Value;
        }

        public void SendEmail(string from, string to, string subject, string message)
        {
            try
            {
                using var client = new SmtpClient();
                using var mimeMessage = new MimeMessage();
                mimeMessage.From.Add(new MailboxAddress("", from));
                mimeMessage.To.Add(new MailboxAddress("", to));
                var bodyBuilder = new BodyBuilder();
                bodyBuilder.HtmlBody = message;

                mimeMessage.Subject = subject;
                mimeMessage.Body = bodyBuilder.ToMessageBody();

                client.Connect(_emailConfiguration.SmtpServer, _emailConfiguration.SmtpPort,true);
                client.AuthenticationMechanisms.Remove("XOAUTH2");
                client.Authenticate(_emailConfiguration.SmtpUsername, _emailConfiguration.SmtpPassword);
                _logger.LogInformation($"sending email to {to}");
                client.Send(mimeMessage);
                client.Disconnect(true);
            }
            catch (Exception)
            {
                _logger.LogError($"Something went wrong while sending email to {to}");
                throw new EmailNotSentException($"Something went wrong while sending email to {to}");
            }

        }
    }
}
