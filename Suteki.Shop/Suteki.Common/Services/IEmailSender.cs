namespace Suteki.Common.Services
{
    public interface IEmailSender
    {
        void Send(string toAddress, string subject, string body);
        void Send(string[] toAddress, string subject, string body);
    }
}