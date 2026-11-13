using System.Globalization;
using System.Net;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Vectarr.Palooza.ApiBusiness.Emailing.BaseEmailTemplating;
using Vectarr.Palooza.ApiBusiness.Emailing.EmailTemplates.TemplateClasses;
using Vectarr.Palooza.ApiBusiness.IServices;
using Vectarr.Palooza.Classes.Api;
using Vectarr.Palooza.CoreClasses.Enums;

namespace Vectarr.Palooza.ApiBusiness.Services.QuoteRequestServices
{
    /// <summary>
    /// Emails sent when a quote request becomes paid: one to each InstantQuote:AdminNotificationEmails address
    /// and a receipt to the customer. Sending runs in the background and never throws, so it can never
    /// break or slow down payment confirmation (failures are logged).
    /// </summary>
    public class QuoteRequestNotificationServices
    {
        private const string Empty = "&mdash;";

        private readonly IEmailSender _emailSender;
        private readonly IConfiguration _configuration;
        private readonly ILogger<QuoteRequestNotificationServices> _logger;

        public QuoteRequestNotificationServices(IEmailSender emailSender, IConfiguration configuration, ILogger<QuoteRequestNotificationServices> logger)
        {
            _emailSender = emailSender;
            _configuration = configuration;
            _logger = logger;
        }

        public void SendPaidNotifications(QuoteRequestModel model)
        {
            try
            {
                var config = _configuration.GetSection(InstantQuoteConfig.SectionName).Get<InstantQuoteConfig>() ?? new InstantQuoteConfig();
                var siteUrl = string.IsNullOrWhiteSpace(config.SiteUrl) ? "https://vectarr.com" : config.SiteUrl.Trim().TrimEnd('/');
                var adminLink = siteUrl + "/admin/quote-requests/" + model.Id.ToString(CultureInfo.InvariantCulture);

                var adminEmails = config.AdminNotificationEmails
                    .Where(e => !string.IsNullOrWhiteSpace(e))
                    .Select(e => e.Trim())
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .ToList();

                var values = BuildTagValues(model, adminLink);
                var reference = model.ReferenceNumber;
                var customerEmail = model.ContactEmail?.Trim();

                // Only singletons (IEmailSender, ILogger) and precomputed values are captured: safe outside the request scope.
                var emailSender = _emailSender;
                var logger = _logger;

                _ = Task.Run(async () =>
                {
                    foreach (var adminEmail in adminEmails)
                    {
                        try
                        {
                            var sender = new EmailTemplateSender(emailSender);
                            await sender.SendAsync(new QuoteRequestPaidAdminEmail(values),
                                new IdentityModel { EmailAddress = adminEmail },
                                overrideSubject: $"New paid instant quote {reference} - Vectarr");
                        }
                        catch (Exception ex)
                        {
                            logger.LogError(ex, "Could not send the paid quote request notification {Reference} to {Email}.", reference, adminEmail);
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(customerEmail))
                    {
                        try
                        {
                            var sender = new EmailTemplateSender(emailSender);
                            await sender.SendAsync(new QuoteRequestReceiptEmail(values),
                                new IdentityModel { EmailAddress = customerEmail },
                                overrideSubject: $"Payment received for your Vectarr quote {reference}");
                        }
                        catch (Exception ex)
                        {
                            logger.LogError(ex, "Could not send the quote request receipt {Reference} to the customer.", reference);
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Could not queue the paid notifications for quote request {Id}.", model.Id);
            }
        }

        /// <summary>
        /// Values are HTML encoded here (the template compiler inserts them as raw HTML) and '$' is doubled
        /// because the compiler uses Regex.Replace.
        /// </summary>
        private static Dictionary<EmailTemplateTagsEnum, string> BuildTagValues(QuoteRequestModel model, string adminLink)
        {
            return new Dictionary<EmailTemplateTagsEnum, string>
            {
                { EmailTemplateTagsEnum.ReferenceNumber, Encode(model.ReferenceNumber) },
                { EmailTemplateTagsEnum.ContactName, Encode(model.ContactName) },
                { EmailTemplateTagsEnum.EmailAddress, Encode(model.ContactEmail) },
                { EmailTemplateTagsEnum.CompanyName, Encode(model.Company) },
                { EmailTemplateTagsEnum.PhoneNumber, Encode(model.Phone) },
                { EmailTemplateTagsEnum.PartName, Encode(model.PartName) },
                { EmailTemplateTagsEnum.PartNumber, Encode(model.PartNumber) },
                { EmailTemplateTagsEnum.Quantity, Encode(model.Quantity.ToString("N0", CultureInfo.InvariantCulture)) },
                { EmailTemplateTagsEnum.MaterialName, Encode(model.MaterialName) },
                { EmailTemplateTagsEnum.ProcessName, Encode(model.ProcessName) },
                { EmailTemplateTagsEnum.PriceTotal, Encode(FormatMoney(model.TotalPrice, model.Currency)) },
                { EmailTemplateTagsEnum.AmountPaid, Encode(FormatMoney(model.AmountPaid ?? model.AmountDue, model.Currency)) },
                { EmailTemplateTagsEnum.AdminLink, Encode(adminLink) }
            };
        }

        private static string Encode(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return Empty;
            }

            return WebUtility.HtmlEncode(value.Trim()).Replace("$", "$$");
        }

        private static string FormatMoney(decimal amount, string? currency)
        {
            var formatted = amount.ToString("N2", CultureInfo.InvariantCulture);
            var code = string.IsNullOrWhiteSpace(currency) ? "usd" : currency.Trim().ToLowerInvariant();
            return code == "usd" ? "$" + formatted + " USD" : formatted + " " + code.ToUpperInvariant();
        }
    }
}
