using System;
using System.Net;
using System.Net.Mail;
using System.Text;
using Suteki.Common.Services;
using Suteki.Common.Extensions;

namespace Suteki.Common.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly string smtpServer;
        private readonly string fromAddress;
        private readonly string username;
        private readonly string password;

        public EmailSender(string smtpServer, string fromAddress)
        {
            if (smtpServer == null) throw new ArgumentNullException("smtpServer");
            if (fromAddress == null) throw new ArgumentNullException("fromAddress");

            this.smtpServer = smtpServer;
            this.fromAddress = fromAddress;
        }

        public EmailSender(string smtpServer, string fromAddress, string username, string password)
        {
            if (smtpServer == null) throw new ArgumentNullException("smtpServer");
            if (fromAddress == null) throw new ArgumentNullException("fromAddress");
            if (username == null) throw new ArgumentNullException("username");
            if (password == null) throw new ArgumentNullException("password");

            this.smtpServer = smtpServer;
            this.fromAddress = fromAddress;
            this.username = username;
            this.password = password;
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

            if (username == null || password == null)
            {
                smtpClient.EnableSsl = true;
                smtpClient.UseDefaultCredentials = true;
            }
            else
            {
                smtpClient.EnableSsl = true;
                smtpClient.Credentials = new NetworkCredential(username, password);
            }

            smtpClient.Send(message);
        }
    }
}