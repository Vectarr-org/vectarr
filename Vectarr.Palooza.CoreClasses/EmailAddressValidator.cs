using System.Text.RegularExpressions;

namespace Vectarr.Palooza.CoreClasses
{
    /// <summary>
    /// Format check for account emails. Any domain and TLD are allowed (.cn, .de, .co.uk, .io, and so on).
    /// Registration only requires a non-empty address; login uses this so those same addresses can sign in.
    /// </summary>
    public static class EmailAddressValidator
    {
        // Local part, one or more domain labels, then a letter TLD of at least two characters.
        // The TLD is not an allowlist.
        private static readonly Regex Pattern = new(
            @"^[^@\s]+@(?:[a-z0-9](?:[a-z0-9-]{0,61}[a-z0-9])?\.)+[a-z]{2,}$",
            RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled);

        public static bool IsValid(string? email)
        {
            if (string.IsNullOrWhiteSpace(email) || email.Length > 254)
            {
                return false;
            }

            return Pattern.IsMatch(email);
        }
    }
}
