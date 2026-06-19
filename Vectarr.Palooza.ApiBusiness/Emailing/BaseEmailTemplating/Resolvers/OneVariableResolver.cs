using Vectarr.Palooza.CoreClasses.Enums;
using Vectarr.Palooza.ApiBusiness.Emailing.IBaseEmailTemplating;

namespace Vectarr.Palooza.ApiBusiness.Emailing.BaseEmailTemplating.Resolvers
{
    public class OneVariableResolver : IEmailTemplateTagResolver
    {
        private readonly EmailTemplateTagsEnum _variable;
        private readonly string _value;

        public OneVariableResolver(EmailTemplateTagsEnum variable, string value)
        {
            _variable = variable;
            _value = value;
        }

        public Dictionary<EmailTemplateTagsEnum, string> ResolveTags()
        {
            return new Dictionary<EmailTemplateTagsEnum, string>
            {
                {_variable, _value }
            };
        }

        public Dictionary<EmailTemplateTagsEnum, List<IEmailTemplateTagResolver>> ResolveIterationsTags()
        {
            return new Dictionary<EmailTemplateTagsEnum, List<IEmailTemplateTagResolver>>();
        }

        public List<EmailTemplateTagsEnum> AllowedTags
        {
            get
            {
                return new List<EmailTemplateTagsEnum>
                {
                    _variable
                };
            }
        }

        public List<EmailTemplateTagsEnum> AllowedRepeatTags
        {
            get
            {
                return new List<EmailTemplateTagsEnum>();
            }
        }
    }
}
