using Vectarr.Palooza.Classes.Api;
using System.Net.Mail;
using Vectarr.Palooza.Classes.Email;

namespace Vectarr.Palooza.ApiBusiness.Emailing.IBaseEmailTemplating
{
    public interface IEmailTemplateSender
    {
        string GetResultBody(IEmailTemplate emailTemplate, IdentityModel recipient);

        void Send(IEmailTemplate emailTemplate, IdentityModel recipient, List<Attachment> attachments = null, List<NotificationEmailModel> ccEmails = null, string overrideSubject = null, string overrideEmail = null);

        Task SendAsync(IEmailTemplate emailTemplate, IdentityModel recipient, List<Attachment> attachments = null, List<NotificationEmailModel> ccEmails = null, string overrideSubject = null);
    }
}
