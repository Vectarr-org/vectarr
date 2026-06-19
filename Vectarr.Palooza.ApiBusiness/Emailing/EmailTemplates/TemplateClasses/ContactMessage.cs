using Vectarr.Palooza.CoreClasses.Enums;
using Vectarr.Palooza.ApiBusiness.Emailing.IBaseEmailTemplating;
using Vectarr.Palooza.ApiBusiness.Emailing.BaseEmailTemplating;

namespace Vectarr.Palooza.ApiBusiness.Emailing.EmailTemplates.TemplateClasses
{
    public class ContactMessage : BaseEmailTemplate
    {
        private readonly string _email;
        private readonly string _name;
        private readonly string _text;
        public ContactMessage(string email, string name, string text) : base("ContactMessageEmail.html")
        {
            _email = email;
            _name = name;
            _text = text;
        }

        public override EmailNotificationTypeEnum NotificationType => EmailNotificationTypeEnum.ContactMessage;

        public override IEmailContextBuilder GetContextBuilder()
        {
            return EmailContextBuilder.ContextBuilder()
                .HasVariable(EmailTemplateTagsEnum.EmailAddress, _email)
                .HasVariable(EmailTemplateTagsEnum.FullName, _name)
                .HasVariable(EmailTemplateTagsEnum.Text, _text);
        }
    }
}
