using System;
using System.Net.Mail;
using System.Text;
using Suteki.Common.Services;
using Suteki.Common.Extensions;

namespace Suteki.Common.Services
{
    public class EmailSender : IEmailSender
    {
        readonly string smtpServer;
        readonly string fromAddress;

        public EmailSender(string smtpServer, string fromAddress)
        {
            if (smtpServer == null) throw new ArgumentNullException("smtpServer");
            if (fromAddress == null) throw new ArgumentNullException("fromAddress");

            this.smtpServer = smtpServer;
            this.fromAddress = fromAddress;
        }

        public void Send(string toAddress, string subject, string body)
        {
            Send(new[] { toAddress }, subject, body);
        }

        public void Send(string[] toAddress, string subject, string body)
        {
            // if the smtpServer is not configured, just return
            if (smtpServer == "") return;

            var message = new MailMessage
                                      {
                                          From = new MailAddress(fromAddress),
                                          Subject = subject,
                                          Body = body,
                                          IsBodyHtml = true
                                      };
            toAddress.ForEach(a => message.To.Add(a));

            var smtpClient = new SmtpClient(smtpServer);
            smtpClient.UseDefaultCredentials = true;
            smtpClient.Send(message);
        }
    }
}