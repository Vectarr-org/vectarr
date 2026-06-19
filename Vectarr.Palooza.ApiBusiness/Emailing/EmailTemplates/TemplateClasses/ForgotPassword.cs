using Vectarr.Palooza.Classes.Api;
using Vectarr.Palooza.CoreClasses.Enums;
using Vectarr.Palooza.ApiBusiness.Emailing.IBaseEmailTemplating;
using Vectarr.Palooza.ApiBusiness.Emailing.BaseEmailTemplating;

namespace Vectarr.Palooza.ApiBusiness.Emailing.EmailTemplates.TemplateClasses
{
    public class ForgotPassword : BaseEmailTemplate
    {
        private readonly IdentityModel _identity;
        private readonly string _resetPasswordLink;
        public ForgotPassword(IdentityModel identity, string resetPasswordLink) : base("ForgotPassword.html")
        {
            _identity = identity;
            _resetPasswordLink = resetPasswordLink;
        }

        public override EmailNotificationTypeEnum NotificationType
        {
            get
            {
                return EmailNotificationTypeEnum.ForgotPassword;
            }
        }

        public override IEmailContextBuilder GetContextBuilder()
        {
            return EmailContextBuilder.ContextBuilder()
                .HasIdentityInfo(_identity)
                .HasVariable(EmailTemplateTagsEnum.ResetPasswordLink, _resetPasswordLink);
        }
    }
}
