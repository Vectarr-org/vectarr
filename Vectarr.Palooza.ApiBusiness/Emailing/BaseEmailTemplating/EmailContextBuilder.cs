using Vectarr.Palooza.ApiBusiness.Emailing.BaseEmailTemplating.Resolvers;
using Vectarr.Palooza.Classes.Api;
using Vectarr.Palooza.CoreClasses.Enums;
using Vectarr.Palooza.ApiBusiness.Emailing.IBaseEmailTemplating;

namespace Vectarr.Palooza.ApiBusiness.Emailing.BaseEmailTemplating
{
    public class EmailContextBuilder : IEmailContextBuilder
    {
        #region Private Fields
        /// <summary>
        /// The tag resolvers
        /// </summary>
        private readonly List<IEmailTemplateTagResolver> _tagResolvers = new List<IEmailTemplateTagResolver>();
        #endregion

        #region Constructor
        /// <summary>
        /// Prevents a default instance of the <see cref="EmailContextBuilder"/> class from being created.
        /// </summary>
        private EmailContextBuilder()
        {
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Starts a new email template context builder
        /// </summary>
        /// <returns>EmailContextBuilder.</returns>
        public static EmailContextBuilder ContextBuilder()
        {
            return new EmailContextBuilder();
        }

        /// <summary>
        /// Specifies that this email context will need identity information
        /// </summary>
        /// <param name="identity">The identity.</param>
        /// <returns>EmailContextBuilder.</returns>
        public EmailContextBuilder HasIdentityInfo(IdentityModel identity)
        {
            _tagResolvers.Add(new IdentityTagResolver(identity));
            return this;
        }

        /// <summary>
        /// Specifies that this email has a hard wired resolver
        /// </summary>
        /// <param name="resolver">The resolver.</param>
        /// <returns>EmailContextBuilder.</returns>
        private EmailContextBuilder HasResolver(IEmailTemplateTagResolver resolver)
        {
            _tagResolvers.Add(resolver);
            return this;
        }

        /// <summary>
        /// Specifies a single variable resolver for this email
        /// </summary>
        /// <param name = "variable" > The variable.</param>
        /// <param name = "value" > The value.</param>
        /// <returns>EmailContextBuilder.</returns>
        public EmailContextBuilder HasVariable(EmailTemplateTagsEnum variable, string value)
        {
            _tagResolvers.Add(new OneVariableResolver(variable, value));
            return this;
        }


        public EmailContextBuilder HasBecomeAPartner(BecomeAPartnerRequest becomeAPartnerRequest)
        {
            _tagResolvers.Add(new BecomeAPartnerTagResolver(becomeAPartnerRequest));
            return this;
        }

        /// <summary>
        /// Resolves all the required info and gets the final tags dictionary
        /// </summary>
        /// <returns>Dictionary&lt;System.String, System.String&gt;.</returns>
        public Dictionary<string, string> GetTags()
        {
            Dictionary<string, string> result = new Dictionary<string, string>();

            foreach (var resolver in _tagResolvers)
            {
                var tags = resolver.ResolveTags();
                foreach (var key in tags.Keys)
                {
                    if (!result.ContainsKey(key.ToString()))
                    {
                        result.Add(key.ToString(), tags[key]);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Gets sub context builders, to be used for iteration nodes
        /// </summary>
        /// <returns>Dictionary&lt;System.String, List&lt;IEmailContextBuilder&gt;&gt;.</returns>
        public Dictionary<string, List<IEmailContextBuilder>> GetIterationTags()
        {
            var result = new Dictionary<string, List<IEmailContextBuilder>>();

            foreach (var tagResolver in _tagResolvers)
            {
                var tags = tagResolver.ResolveIterationsTags();
                if (tags != null && tags.Any())
                {
                    foreach (var tag in tags)
                    {
                        result.Add(tag.Key.ToString(), tag.Value.Select(t => ContextBuilder().HasResolver(t)).Cast<IEmailContextBuilder>().ToList());
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Gets a list of the allowed tags to be used in the template
        /// </summary>
        /// <value>The allowed tags.</value>
        public List<string> AllowedTags
        {
            get { return _tagResolvers.SelectMany(e => e.AllowedTags).Distinct().Select(e => e.ToString()).ToList(); }
        }

        /// <summary>
        /// Gets a list of the allowed repeat tags to be used in the template
        /// </summary>
        /// <value>The allowed repeat tags.</value>
        public List<string> AllowedRepeatTags
        {
            get
            {
                return _tagResolvers.SelectMany(e => e.AllowedRepeatTags).Distinct().Select(e => e.ToString()).ToList();
            }
        }
        #endregion
    }
}
