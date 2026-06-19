using Vectarr.Palooza.CoreClasses.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vectarr.Palooza.ApiBusiness.Emailing.IBaseEmailTemplating
{
    public interface IEmailTemplate
    {
        IEmailContextBuilder GetContextBuilder();

        string GetDefaultTemplate();

        string GetTemplate(out string subject);

        EmailNotificationTypeEnum NotificationType { get; }

        string Subject { get; }

        bool HasDynamicSubject { get; }

        bool AddHeaderAndFooter { get; }

        bool AllowAddingBccEmails { get; }
        string GetHeader();
        string GetFooter();
    }
}
