using Vectarr.Palooza.Classes.Api;

namespace Vectarr.Palooza.Classes.Privacy
{
    /// <summary>
    /// Workshop location rules. City, state, and ZIP are required.
    /// Street is optional. Phone is required (account verification and backup contact)
    /// and is stripped from buyer responses by <see cref="MarketplacePrivacy"/>.
    /// </summary>
    public static class WorkshopLocationRules
    {
        public static IReadOnlyList<string> Validate(AddressModel? address, string? phoneNumber)
        {
            var errors = new List<string>();
            if (string.IsNullOrWhiteSpace(phoneNumber))
            {
                errors.Add("Phone number is required.");
            }

            if (address == null)
            {
                errors.Add("City, state, and ZIP code are required.");
                return errors;
            }

            if (string.IsNullOrWhiteSpace(address.City))
            {
                errors.Add("City is required.");
            }

            if (string.IsNullOrWhiteSpace(address.State))
            {
                errors.Add("State is required.");
            }

            if (string.IsNullOrWhiteSpace(address.PostalCode))
            {
                errors.Add("ZIP code is required.");
            }

            return errors;
        }
    }
}
