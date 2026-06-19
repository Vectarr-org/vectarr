using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Vectarr.Palooza.ApiBusiness.Services.ImageServices
{
    public class SEO
    {
        // white space, em-dash, en-dash, underscore
        static readonly Regex WordDelimiters = new Regex(@"[\s—–_]", RegexOptions.Compiled);

        // characters that are not valid
        static readonly Regex InvalidChars = new Regex(@"[^\w\-]", RegexOptions.Compiled);

        // multiple hyphens
        static readonly Regex MultipleHyphens = new Regex(@"-{2,}", RegexOptions.Compiled);

        public static string ToSeoUrl(string value)
        {
            if (value == null)
            {
                return null;
            }

            // convert to lower case
            value = value.ToLowerInvariant();

            // remove diacritics (accents)
            value = RemoveDiacritics(value);

            // ensure all word delimiters are hyphens
            value = WordDelimiters.Replace(value, "-");

            // strip out invalid characters
            value = InvalidChars.Replace(value, "");

            // replace multiple hyphens (-) with a single hyphen
            value = MultipleHyphens.Replace(value, "-");

            // trim hyphens (-) from ends
            return value.Trim('-');
        }

        /// See: http://blogs.msdn.com/b/michkap/archive/2007/05/14/2629747.aspx
        private static string RemoveDiacritics(string expression)
        {
            string normalizedString = expression.Normalize(NormalizationForm.FormD);
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < normalizedString.Length; i++)
            {
                UnicodeCategory uc = CharUnicodeInfo.GetUnicodeCategory(normalizedString[i]);
                if (uc != UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(normalizedString[i]);
                }
            }

            return sb.ToString().Normalize(NormalizationForm.FormC);
        }
    }
}
