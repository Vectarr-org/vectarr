using Vectarr.Palooza.ApiBusiness.Emailing.BaseEmailTemplating;
using Vectarr.Palooza.ApiBusiness.Emailing.IBaseEmailTemplating;
using Vectarr.Palooza.CoreClasses.Enums;

namespace Vectarr.Palooza.ApiBusiness.Emailing.EmailTemplates.TemplateClasses
{
    /// <summary>
    /// Receipt sent to the customer when their instant quote request is paid.
    /// The values must already be HTML encoded.
    /// </summary>
    public class QuoteRequestReceiptEmail : BaseEmailTemplate
    {
        private readonly IReadOnlyDictionary<EmailTemplateTagsEnum, string> _values;

        public QuoteRequestReceiptEmail(IReadOnlyDictionary<EmailTemplateTagsEnum, string> values) : base("QuoteRequestReceipt.html")
        {
            _values = values;
        }

        public override EmailNotificationTypeEnum NotificationType => EmailNotificationTypeEnum.QuoteRequestReceipt;

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
