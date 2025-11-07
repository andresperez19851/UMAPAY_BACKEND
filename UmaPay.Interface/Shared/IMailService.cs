using System.Net.Mail;

namespace UmaPay.Interface.Shared
{

    public interface IMailService : IDisposable
    {
        Task SendAsync(string to, string subject, string body);
    }
}