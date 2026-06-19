using Vectarr.Palooza.Classes.Api;
using Vectarr.Palooza.CoreClasses.Enums;
using Vectarr.Palooza.ApiBusiness.Emailing.IBaseEmailTemplating;
using Vectarr.Palooza.ApiBusiness.Emailing.BaseEmailTemplating;

namespace Vectarr.Palooza.ApiBusiness.Emailing.EmailTemplates.TemplateClasses
{
    public class EmailVerification : BaseEmailTemplate
    {
        private readonly string _verificationLink;
        private readonly IdentityModel _user;
        public EmailVerification(IdentityModel user, string verificationLink) : base("EmailVerification.html")
        {
            _user = user;
            _verificationLink = verificationLink;
        }

        public override EmailNotificationTypeEnum NotificationType
        {
            get
            {
                return EmailNotificationTypeEnum.EmailVerification;
            }
        }

        public override IEmailContextBuilder GetContextBuilder()
        {
            return EmailContextBuilder.ContextBuilder()
                .HasIdentityInfo(_user)
                .HasVariable(EmailTemplateTagsEnum.RegisterVerificationLink, _verificationLink);
        }
    }
}
