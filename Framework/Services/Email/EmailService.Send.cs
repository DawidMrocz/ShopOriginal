using Framework.Configuration;
using Framework.Helpers;
using Framework.Models.Template;
using HtmlAgilityPack;
using Microsoft.EntityFrameworkCore;
using RestSharp;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Text.RegularExpressions;

namespace Framework.Shared.Services.EmailService
{
    internal partial class EmailService<TDbContext>
    {
        public async Task SendEmail(
            IEnumerable<MailAddress> to,
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
            MailPriority emailPriority = MailPriority.Normal
            )
        {
            TemplateModel? templateMailAddress = await _dbSet.Include(c => c.Culture).FirstOrDefaultAsync(t => t.Name == templateString && t.Culture.Name == culture)
                ?? throw new Exception("Template not found");

            if (templateMailAddress.Body is null || templateMailAddress.Subject is null) throw new Exception("Subject or body not provided");

            //PODMIANA PARAMETRÓW BODY
            if (bodyParams is not null)
                foreach (KeyValuePair<string, string> parameter in bodyParams)
                    templateMailAddress.Body = templateMailAddress.Body.Replace("{{" + parameter.Key + "}}", parameter.Value);

            //PODMIANA PARAMETRÓW SUBJECT
            if (subjectParams is not null)
                foreach (KeyValuePair<string, string> parameter in subjectParams)
                    templateMailAddress.Subject = templateMailAddress.Subject.Replace("{{" + parameter.Key + "}}", parameter.Value);

            using SmtpClient smtpClient = new()
            {
                Host = FrameworkConfiguration.EmailServer,
                Port = FrameworkConfiguration.EmailPort,
                Timeout = 12000,
                EnableSsl = FrameworkConfiguration.EnableSSL,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(FrameworkConfiguration.User, FrameworkConfiguration.Password)
            };
            using MailMessage message = new()
            {
                SubjectEncoding = Encoding.UTF8,
                IsBodyHtml = bodyAsHtml,
                BodyEncoding = Encoding.UTF8,
                Priority = emailPriority,
                Subject = SanitizeBody(templateMailAddress.Subject),
                Body = SanitizeBody(templateMailAddress.Body),
                From = from,
            };
            foreach (MailAddress receiver in to) message.To.Add(receiver);
            if (cc is not null) foreach (MailAddress item in cc) message.CC.Add(item);
            if (bcc is not null) foreach (MailAddress item in bcc) message.Bcc.Add(item);
            if (replyTo is not null) foreach (MailAddress item in replyTo) message.ReplyToList.Add(item);

            //DODANIE ZAŁĄCZNIKÓW
            if (attachmets is not null)
                foreach (Attachment attachmet in attachmets)
                    message.Attachments.Add(new Attachment(attachmet.ContentStream, attachmet.Name));

            //PODMIANA OBRAZKÓW W BODY JEŻELI BODY TO HTML
            if (bodyAsHtml) EmailService<TDbContext>.SetEmbeddedImages(message);
            //----------------------------------------------------------------------------------------------

            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                ServicePointManager.DefaultConnectionLimit = 100;
                ServicePointManager.ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => { return true; };
                smtpClient.Send(message);
            }
            catch (SmtpFailedRecipientsException ex)
            {
                foreach (SmtpFailedRecipientException innerEx in ex.InnerExceptions)
                {
                    SmtpStatusCode status = innerEx.StatusCode;
                    if (status is SmtpStatusCode.MailboxBusy or SmtpStatusCode.MailboxUnavailable)
                    {
                        Thread.Sleep(5000);
                        smtpClient.Send(message);
                    }
                    else
                    {
                        Console.WriteLine($"Failed to deliver message to {innerEx.FailedRecipient}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Mail sending error: {ex.Message}");
            }
        }

        public static string SanitizeBody(string body)
        {
            var result = HtmlAndSvgSanitizer.MoveCssInline(body);
            return HtmlAndSvgSanitizer.SanitizeText(result);
        }

        private static void SetEmbeddedImages(MailMessage mailMessage)
        {
            string body = mailMessage.Body;
            HtmlDocument html = new();
            html.LoadHtml(body);
            HtmlNodeCollection images = html.DocumentNode.SelectNodes("//img");
            if (images != null && images.Any())
            {
                List<LinkedResource> linkedResources = new();
                foreach (HtmlNode? image in images)
                {
                    string base64Guid = image.Attributes["src"].Value.Split(',').Last();
                    if (string.IsNullOrEmpty(base64Guid)) continue;

                    byte[] fileBytes;
                    if (Regex.IsMatch(base64Guid, "^[a-zA-Z0-9+/]*={0,2}$"))
                    {
                        fileBytes = Convert.FromBase64String(base64Guid);
                    }
                    else
                    {
                        RestClient client = new();
                        RestRequest request = new(base64Guid, Method.Get);
                        RestResponse response = client.Execute(request);
                        if (!response.IsSuccessful) throw new Exception("Communication error...");
                        if (response.RawBytes is null) throw new Exception("Problem with image link");
                        fileBytes = response.RawBytes;
                    }
                    string contentIdGuid = Guid.NewGuid().ToString().Replace("-", "");
                    LinkedResource linkedResource = new(new MemoryStream(fileBytes));
                    linkedResource.ContentId = contentIdGuid;

                    Attachment att = new("C:\\xxx.png");
                    att.ContentDisposition!.Inline = true;
                    att.ContentId = contentIdGuid;
                    linkedResources.Add(linkedResource);
                    image.Attributes["src"].Value = $@"cid:{contentIdGuid}";
                    mailMessage.Attachments.Add(att);
                }
                mailMessage.Body = html.DocumentNode.OuterHtml;

                AlternateView? view = AlternateView.CreateAlternateViewFromString(mailMessage.Body, null, MediaTypeNames.Text.Html);
                linkedResources.ForEach(x => view.LinkedResources.Add(x));
                mailMessage.AlternateViews.Add(view);
            }
        }
    }
}
