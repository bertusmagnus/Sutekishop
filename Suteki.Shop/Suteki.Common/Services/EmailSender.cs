using System;
using System.Net.Mail;
using Suteki.Common.Services;
using Suteki.Common.Extensions;

namespace Suteki.Common.Services
{
    public class EmailSender : IEmailSender
    {
        readonly string smtpServer;
    	int port = 25;
        readonly string fromAddress;

    	public int Port
    	{
			get { return port; }
			set { port = value; }
    	}

        public EmailSender(string smtpServer, string fromAddress)
        {
            this.smtpServer = smtpServer;
        	this.fromAddress = fromAddress;
        }

		[Obsolete("Please use the overload with the isBodyHtml parameter. This method defaults that parameter to false")]
        public void Send(string toAddress, string subject, string body)
        {
            Send(new[] { toAddress }, subject, body, false);
        }

		[Obsolete("Please use the overload with the isBodyHtml parameter. This method defaults that parameter to false")]
		public void Send(string[] toAddress, string subject, string body)
		{
			Send(toAddress, subject, body, false);
		}

		public void Send(string toAddress, string subject, string body, bool bodyIsHtml)
		{
			Send(new[] { toAddress }, subject, body, bodyIsHtml);
		}

        public void Send(string[] toAddress, string subject, string body, bool bodyIsHtml)
        {
            // if the smtpServer is not configured, just return
            if (smtpServer == "") return;

            var message = new MailMessage
                                      {
                                          From = new MailAddress(fromAddress),
                                          Subject = subject,
                                          Body = body,
                                          IsBodyHtml = bodyIsHtml,
                                      };
            toAddress.ForEach(a => message.To.Add(a));

            var smtpClient = new SmtpClient(smtpServer) {Port = port};
        	smtpClient.Send(message);
        }
    }
}