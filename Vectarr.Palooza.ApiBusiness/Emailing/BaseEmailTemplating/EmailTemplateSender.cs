using Vectarr.Palooza.DataAccess;
using Vectarr.Palooza.ApiBusiness.IServices;

namespace Vectarr.Palooza.ApiBusiness.Emailing.BaseEmailTemplating
{
    public class EmailTemplateSender : BaseEmailTemplateSender
    {
        private readonly IUnitOfWork _unitOfWork;

        public EmailTemplateSender(IEmailSender emailSender) : base(emailSender)
        {

        }
    }
}
