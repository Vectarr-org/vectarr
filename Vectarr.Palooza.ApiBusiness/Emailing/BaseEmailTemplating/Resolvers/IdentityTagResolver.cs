using Vectarr.Palooza.Classes.Api;
using Vectarr.Palooza.CoreClasses.Enums;
using Vectarr.Palooza.ApiBusiness.Emailing.IBaseEmailTemplating;

namespace Vectarr.Palooza.ApiBusiness.Emailing.BaseEmailTemplating.Resolvers
{
    public class IdentityTagResolver : IEmailTemplateTagResolver
    {
        private readonly IdentityModel _identity;

        public IdentityTagResolver(IdentityModel identity)
        {
            _identity = identity;
        }

        public Dictionary<EmailTemplateTagsEnum, string> ResolveTags()
        {
            Dictionary<EmailTemplateTagsEnum, string> result = new Dictionary<EmailTemplateTagsEnum, string>
            {
                {EmailTemplateTagsEnum.FullName, _identity.FirstName + " " + _identity.LastName},
                {EmailTemplateTagsEnum.FullnameOrEmailAddress , string.IsNullOrEmpty(_identity.FirstName) ? _identity.EmailAddress : _identity.FirstName + " " + _identity.LastName},
                {EmailTemplateTagsEnum.EmailAddress, _identity.EmailAddress}
            };

            return result;
        }

        public Dictionary<EmailTemplateTagsEnum, List<IEmailTemplateTagResolver>> ResolveIterationsTags()
        {
            return null;
        }

        public List<EmailTemplateTagsEnum> AllowedTags
        {
            get
            {
                return new List<EmailTemplateTagsEnum>
                {
                    EmailTemplateTagsEnum.FirstName,
                    EmailTemplateTagsEnum.LastName,
                    EmailTemplateTagsEnum.FullName,
                    EmailTemplateTagsEnum.EmailAddress
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
