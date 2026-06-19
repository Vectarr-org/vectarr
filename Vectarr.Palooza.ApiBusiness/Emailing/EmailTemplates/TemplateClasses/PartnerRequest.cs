using Vectarr.Palooza.ApiBusiness.Emailing.BaseEmailTemplating;
using Vectarr.Palooza.ApiBusiness.Emailing.IBaseEmailTemplating;
using Vectarr.Palooza.Classes.Api;
using Vectarr.Palooza.CoreClasses.Enums;

namespace Vectarr.Palooza.ApiBusiness.Emailing.EmailTemplates.TemplateClasses
{
    internal class PartnerRequest : BaseEmailTemplate
    {

        private readonly BecomeAPartnerRequest _request;
        public PartnerRequest(BecomeAPartnerRequest request) : base("PartnerRequest.html")
        {
            _request = request;
        }

        public override EmailNotificationTypeEnum NotificationType => EmailNotificationTypeEnum.ParnerRequest;

        public override IEmailContextBuilder GetContextBuilder()
        {
            return EmailContextBuilder.ContextBuilder().HasBecomeAPartner(_request);
        }
    }
}
