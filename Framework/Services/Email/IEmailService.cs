using System.Net.Mail;

namespace Framework.Services.Email
{
    public interface IEmailService
    {
        Task SendEmail(IEnumerable<MailAddress> to,
            string templateString,
            bool bodyAsHtml,
            string culture,
            MailAddress from,
            Dictionary<string, string>? bodyParams = null,
            Dictionary<string, string>? subjectParams = null,
            IEnumerable<Attachment>? attachmets = null,
            IEnumerable<MailAddress>? cc = null,
            IEnumerable<MailAddress>? bcc = null,
            IEnumerable<MailAddress>? replyTo = null,
            MailPriority emailPriority = MailPriority.Normal);
    }
}
