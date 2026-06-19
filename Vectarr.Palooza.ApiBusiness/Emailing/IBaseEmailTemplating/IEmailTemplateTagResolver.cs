using Vectarr.Palooza.CoreClasses.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vectarr.Palooza.ApiBusiness.Emailing.IBaseEmailTemplating
{
    public interface IEmailTemplateTagResolver
    {
        /// <summary>
        /// Gets a list of tags and their replacements
        /// </summary>
        /// <returns></returns>
        Dictionary<EmailTemplateTagsEnum, string> ResolveTags();

        /// <summary>
        /// Gets a list of repeat tags and the resolvers they should use
        /// </summary>
        /// <returns></returns>
        Dictionary<EmailTemplateTagsEnum, List<IEmailTemplateTagResolver>> ResolveIterationsTags();

        /// <summary>
        /// Gets a list of the allowed tags for this resolver
        /// </summary>
        List<EmailTemplateTagsEnum> AllowedTags { get; }

        /// <summary>
        /// Gets a list of the allowed repeat tags for this resolver
        /// </summary>
        List<EmailTemplateTagsEnum> AllowedRepeatTags { get; }
    }
}
