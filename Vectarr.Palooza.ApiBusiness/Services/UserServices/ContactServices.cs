using Vectarr.Palooza.Classes.Api;
using Vectarr.Palooza.Exceptions;
using Vectarr.Palooza.ApiBusiness.IServices;
using Vectarr.Palooza.ApiBusiness.Emailing.EmailTemplates.TemplateClasses;
using Vectarr.Palooza.ApiBusiness.Emailing.BaseEmailTemplating;

namespace Vectarr.Palooza.ApiBusiness.Services.UserServices
{
    public class ContactServices
    {
        private readonly IEmailSender _emailSender;
        public ContactServices(IEmailSender emailSender)
        {
            _emailSender = emailSender;
        }

        public async Task SendContactEmail(ContactModel contact)
        {
            if (string.IsNullOrEmpty(contact.Email) || string.IsNullOrEmpty(contact.Name) || string.IsNullOrEmpty(contact.Text))
            {
                throw new ErrorValidationException("All fields are required.");
            }

            EmailTemplateSender sender = new EmailTemplateSender(_emailSender);
            var userModel = new IdentityModel() { EmailAddress = "info@Vectarr.com" };
            ContactMessage contactMessage = new ContactMessage(contact.Email, contact.Name, contact.Text);
            await sender.SendAsync(contactMessage, userModel, overrideSubject: "New Contact - Vectarr");
        }
    }
}
