using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Vectarr.Palooza.ApiBusiness.IServices;

namespace Vectarr.Palooza.ApiBusiness.Emailing
{
    public class Smtp2GoEmailSender : IEmailSender
    {
        private readonly string _smtpHost = "mail.smtp2go.com"; // or smtp.smtp2go.com
        private readonly int _smtpPort = 587; // 587 (TLS), 465 (SSL), or 25
        private readonly string _apiKey = "api-EEDB406CC7984B9C982BB6F83F02AAF8"; // 🔑 Your SMTP2GO API Key

        public void Send(MailMessage message)
        {
            using (var client = CreateClient())
            {
                client.Send(message);
            }
        }

        public async Task SendAsync(MailMessage message)
        {
            using (var client = CreateClient())
            {
                await client.SendMailAsync(message);
            }
        }

        private SmtpClient CreateClient()
        {
            return new SmtpClient(_smtpHost, _smtpPort)
            {
                Credentials = new NetworkCredential("vectarr.com", "J6Zlo8lRzjc6XDbM"), // API key used as both username & password
                EnableSsl = true
            };
        }
    }
}
