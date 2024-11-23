using MimeKit;
using System.Net.Mail;

namespace ResearchManagementSystem.Areas.CreSys.Interfaces
{
    public interface IEthicsEmailService
    {
        Task SendEmailAsync(string toEmail, string recipientName, string subject, string body);
        Task SendEmailWithAttachmentsAsync(string toEmail, string recipientName, string subject, string body, List<MimePart> attachments);
    }
}
