using System.Net.Mail;
using Vectarr.Palooza.Classes.Api;
using Vectarr.Palooza.Classes.Email;
using Vectarr.Palooza.DataAccess;
using Vectarr.Palooza.ApiBusiness.Emailing.IBaseEmailTemplating;
using Vectarr.Palooza.ApiBusiness.IServices;

namespace Vectarr.Palooza.ApiBusiness.Emailing.BaseEmailTemplating
{
    public abstract class BaseEmailTemplateSender : IEmailTemplateSender
    {
        private readonly IEmailSender _emailSender;
        private readonly IEmailTemplateCompiler _compiler;

        protected BaseEmailTemplateSender(IEmailSender emailSender)
        {
            _emailSender = emailSender;
            _compiler = new EmailTemplateCompiler();
        }

        public void Send(IEmailTemplate emailTemplate, IdentityModel recipient, List<Attachment> attachments = null, List<NotificationEmailModel> ccEmails = null, string overrideSubject = null, string overrideEmail = null)
        {
            var message = CreateMessage(emailTemplate, recipient, attachments, ccEmails);
            message.Subject = overrideSubject ?? message.Subject;
            if (!string.IsNullOrEmpty(overrideEmail))
            {
                message.To.Clear();
                message.To.Add(overrideEmail);
            }
            _emailSender.Send(message);
        }

        public async Task SendAsync(IEmailTemplate emailTemplate, IdentityModel recipient, List<Attachment> attachments = null, List<NotificationEmailModel> ccEmails = null, string overrideSubject = null)
        {
            var message = CreateMessage(emailTemplate, recipient, attachments, ccEmails);
            message.Subject = overrideSubject ?? message.Subject;
            await _emailSender.SendAsync(message);
        }

        /// <summary>
        /// Gets the email html, specifying the body. Useful for previewing
        /// </summary>
        /// <param name="emailTemplate"></param>
        /// <param name="recipient">The identity of the recipient, mainly used for header and footer</param>
        /// <param name="template">The html string of the template to use</param>
        /// <param name="storeId"></param>
        /// <returns></returns>
        public string GetResultBody(IEmailTemplate emailTemplate, IdentityModel recipient)
        {
            return GetEmailHtml(emailTemplate, recipient);
        }

        /// <summary>
        /// Gets the email html. Takes the body from the database
        /// </summary>
        /// <param name="emailTemplate"></param>
        /// <param name="recipient">The identity of the recipient, mainly used for header and footer</param>
        /// <param name="subject">The result subject</param>
        /// <param name="storeId"></param>
        /// <param name="databaseTemplate"></param>
        /// <returns></returns>
        private string GetResultBody(IEmailTemplate emailTemplate, IdentityModel recipient, out string subject, EmailTemplateModel databaseTemplate)
        {
            var template = emailTemplate.GetDefaultTemplate();
            subject = "Subject"; /*_compiler.ReplaceTagsFromString(subject, emailTemplate.GetContextBuilder());*/
            return GetEmailHtml(emailTemplate, recipient);
        }

        /// <summary>
        /// Internal method to get the email full html, including the header and the footer
        /// </summary>
        /// <param name="emailTemplate"></param>
        /// <param name="recipient">The identity of the recipient, mainly used for header and footer</param>
        /// <param name="bodyTemplate">The html of the template body</param>
        /// <param name="storeId"></param>
        /// <returns></returns>
        private string GetEmailHtml(IEmailTemplate emailTemplate, IdentityModel recipient)
        {
            var body = emailTemplate.GetDefaultTemplate();
            var header = emailTemplate.GetHeader();
            var footer = emailTemplate.GetFooter();

            var bodyHtml = _compiler.CompileTemplate(body, emailTemplate.GetContextBuilder());
            var html = header + bodyHtml + footer;
            return html;
        }

        private MailMessage CreateMessage(IEmailTemplate emailTemplate, IdentityModel recipient, List<Attachment> attachments = null, List<NotificationEmailModel> ccEmails = null)
        {
            var html = GetResultBody(emailTemplate, recipient);
            MailMessage message = new MailMessage();
            if (!string.IsNullOrEmpty(recipient.EmailAddress))
            {
                message.To.Add(recipient.EmailAddress);
            }
            message.Subject = emailTemplate.Subject;
            message.IsBodyHtml = true;
            message.Body = html;
            message.From = new MailAddress("info@vectarr.com", "Vectarr");

            if (attachments != null && attachments.Any())
            {
                attachments.ForEach(attachment => message.Attachments.Add(attachment));
            }

            if (ccEmails != null && ccEmails.Any())
            {
                foreach (var email in ccEmails)
                {
                    message.CC.Add(new MailAddress(email.Email, email.Name));
                }
            }

            return message;
        }
    }
}
