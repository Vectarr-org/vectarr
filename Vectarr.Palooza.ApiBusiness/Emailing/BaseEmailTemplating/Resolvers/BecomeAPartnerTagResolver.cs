using Vectarr.Palooza.CoreClasses.Enums;
using Vectarr.Palooza.ApiBusiness.Emailing.IBaseEmailTemplating;
using Vectarr.Palooza.Classes.Api;

namespace Vectarr.Palooza.ApiBusiness.Emailing.BaseEmailTemplating.Resolvers
{
    public class BecomeAPartnerTagResolver : IEmailTemplateTagResolver
    {
        private readonly BecomeAPartnerRequest _request;

        public BecomeAPartnerTagResolver(BecomeAPartnerRequest request)
        {
            _request = request;
        }

        public Dictionary<EmailTemplateTagsEnum, string> ResolveTags()
        {
            Dictionary<EmailTemplateTagsEnum, string> result = new Dictionary<EmailTemplateTagsEnum, string>
            {
                { EmailTemplateTagsEnum.CompanyName, _request.CompanyName },
                { EmailTemplateTagsEnum.ContactName, _request.ContactName },
                { EmailTemplateTagsEnum.EmailAddress, _request.EmailAddress },
                { EmailTemplateTagsEnum.PhoneNumber, _request.PhoneNumber },
      { EmailTemplateTagsEnum.ManufacturingCapabilities, string.Join(", ", _request.ManufacturingCapabilities.Select(c => c switch
{
    ManufacturingCapability.CncMachining => "CNC Machining",
    ManufacturingCapability.ThreeDPrinting => "3D Printing",
    ManufacturingCapability.SheetMetal => "Sheet Metal",
    ManufacturingCapability.InjectionMolding => "Injection Molding",
    _ => c.ToString()
})) },
                { EmailTemplateTagsEnum.AboutYourShop, _request.AboutYourShop ?? string.Empty }
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
                    EmailTemplateTagsEnum.CompanyName,
                    EmailTemplateTagsEnum.ContactName,
                    EmailTemplateTagsEnum.EmailAddress,
                    EmailTemplateTagsEnum.PhoneNumber,
                    EmailTemplateTagsEnum.ManufacturingCapabilities,
                    EmailTemplateTagsEnum.AboutYourShop
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