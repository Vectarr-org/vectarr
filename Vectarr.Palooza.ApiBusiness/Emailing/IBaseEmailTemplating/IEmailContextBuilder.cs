using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vectarr.Palooza.ApiBusiness.Emailing.IBaseEmailTemplating
{
    public interface IEmailContextBuilder
    {
        /// <summary>
        /// Resolves all the required info and gets the final tags dictionary
        /// </summary>
        /// <returns></returns>
        Dictionary<string, string> GetTags();

        /// <summary>
        /// Gets a sub list of iteration builders that can be used as their own context builders
        /// </summary>
        /// <returns></returns>
        Dictionary<string, List<IEmailContextBuilder>> GetIterationTags();

        /// <summary>
        /// Gets a list of the allowed tags to be used in the template
        /// </summary>
        List<string> AllowedTags { get; }

        /// <summary>
        /// Gets a list of the allowed repeat tags to be used in the template
        /// </summary>
        List<string> AllowedRepeatTags { get; }
    }
}
