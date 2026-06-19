using Vectarr.Palooza.CoreClasses.Enums;
using Vectarr.Palooza.CoreClasses.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Vectarr.Palooza.ApiBusiness.Emailing.IBaseEmailTemplating;

namespace Vectarr.Palooza.ApiBusiness.Emailing.BaseEmailTemplating
{
    public abstract class BaseEmailTemplate : IEmailTemplate
    {

        private readonly string _defaultHtmlFileName;

        protected BaseEmailTemplate(string defaultHtmlFileName)
        {
            _defaultHtmlFileName = defaultHtmlFileName;
        }

        public abstract EmailNotificationTypeEnum NotificationType { get; }

        public virtual string Subject { get; set; }

        public virtual bool HasDynamicSubject => false;

        public virtual bool AddHeaderAndFooter => true;

        public virtual bool AllowAddingBccEmails => true;

        public abstract IEmailContextBuilder GetContextBuilder();

        public string GetDefaultTemplate()
        {
            return ReadFileData(_defaultHtmlFileName);
        }

        public string GetHeader()
        {
            return ReadFileData("Header.html");
        }

        public string GetFooter()
        {
            return ReadFileData("Footer.html");
        }

        private string ReadFileData(string fileName)
        {
            var defaultHtmlFileName = _defaultHtmlFileName;

            if (string.IsNullOrWhiteSpace(_defaultHtmlFileName)) return string.Empty;

            var resourceName = $"Vectarr.Palooza.ApiBusiness.Emailing.EmailTemplates." + fileName;

            string body;

            var assembly = Assembly.GetExecutingAssembly();
            using (var stream = assembly.GetManifestResourceStream(resourceName))
            {
                if (stream == null) throw new Exception($"Could not find resource stream: {resourceName} in assembly: {assembly.FullName}");
                using (var sr = new StreamReader(stream))
                {
                    body = sr.ReadToEnd();
                }
            }

            return body;
        }

        public string GetTemplate(out string subject)
        {
            var result = GetDefaultTemplate();
            subject = new EmailNotificationType().GetDisplayText(NotificationType);
            return $"<div>{result}</div>";
        }
    }
}
