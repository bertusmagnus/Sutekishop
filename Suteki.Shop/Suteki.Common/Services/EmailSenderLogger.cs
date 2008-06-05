using Castle.Core.Logging;
using Suteki.Common.Extensions;

namespace Suteki.Common.Services
{
    public class EmailSenderLogger : IEmailSender
    {
        readonly private IEmailSender emailSender;
        private readonly ILogger logger;

        public EmailSenderLogger(IEmailSender emailSender, ILogger logger)
        {
            this.emailSender = emailSender;
            this.logger = logger;
        }

        public void Send(string toAddress, string subject, string body)
        {
            LogEmail(new[]{toAddress}, subject, body);
            emailSender.Send(toAddress, subject, body);
        }

        public void Send(string[] toAddress, string subject, string body)
        {
            LogEmail(toAddress, subject, body);
            emailSender.Send(toAddress, subject, body);
        }

        private void LogEmail(string[] toAddress, string subject, string body)
        {
            string message = "Email sent to: {0}\r\nSubject: {1}\r\n{2}".With(
                toAddress.Join(";"),
                subject,
                body);
            logger.Info(message);
        }
    }
}
