using Vectarr.Palooza.ApiBusiness.Emailing.BaseEmailTemplating;
using Vectarr.Palooza.ApiBusiness.Emailing.IBaseEmailTemplating;
using Vectarr.Palooza.CoreClasses.Enums;

namespace Vectarr.Palooza.ApiBusiness.Emailing.EmailTemplates.TemplateClasses
{
    /// <summary>
    /// Sent to InstantQuote:AdminNotificationEmails when an instant quote request is paid.
    /// The values must already be HTML encoded.
    /// </summary>
    public class QuoteRequestPaidAdminEmail : BaseEmailTemplate
    {
        private readonly IReadOnlyDictionary<EmailTemplateTagsEnum, string> _values;

        public QuoteRequestPaidAdminEmail(IReadOnlyDictionary<EmailTemplateTagsEnum, string> values) : base("QuoteRequestPaidAdmin.html")
        {
            _values = values;
        }

        public override EmailNotificationTypeEnum NotificationType => EmailNotificationTypeEnum.QuoteRequestPaidAdmin;

        public override IEmailContextBuilder GetContextBuilder()
        {
            var builder = EmailContextBuilder.ContextBuilder();
            foreach (var value in _values)
            {
                builder.HasVariable(value.Key, value.Value);
            }
            return builder;
        }
    }
}
