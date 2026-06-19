using Vectarr.Palooza.CoreClasses.Enums;
using Vectarr.Palooza.CoreClasses.Wrappers;

namespace Vectarr.Palooza.Classes.Email
{
    public class EmailTemplateModel
    {
        public EmailNotificationTypeEnum NotificationType { get; set; }
        public string NotificationTypeText => new EmailNotificationType().GetDisplayText(NotificationType);

        public string Subject { get; set; }

        public string Body { get; set; }

        public int StoreId { get; set; }

        public ICollection<string> AllowedVariables { get; set; }

        public ICollection<string> AllowedRepeatVariables { get; set; }

        public IEnumerable<NotificationEmailModel> NotificationEmailBccs { get; set; }

        public string DefaultTemplateBody { get; set; }

        public bool SubjectRequired { get; set; }
        public bool AllowAddingBccEmails { get; set; }

        public bool SendToMerchantBcc { get; set; }

        public bool UseDefaultBody { get; set; }

        public bool Enable { get; set; }
    }
}
