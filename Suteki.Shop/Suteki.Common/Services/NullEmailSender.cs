namespace Suteki.Common.Services
{
    public class NullEmailSender : IEmailSender
    {
        public void Send(string toAddress, string subject, string body)
        {
            // do nothing
        }

        public void Send(string[] toAddress, string subject, string body)
        {
            // do nothing
        }
    }
}
