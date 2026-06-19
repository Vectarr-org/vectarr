using Microsoft.AspNetCore.Mvc;
using Vectarr.Palooza.ApiBusiness.IServices;
using Vectarr.Palooza.ApiBusiness.Services.WorkOrderServices;
using Vectarr.Palooza.Classes.Api;
using Vectarr.Palooza.DataAccess;

namespace Vectarr.Palooza.WebApi.Controllers
{
    [Route("api/[controller]")]
    public class ChatMessageController : BaseController<ChatMessageModel>
    {
        private readonly ChatMessageServices _chatMessageServices;
        public ChatMessageController(IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor) : base(unitOfWork, httpContextAccessor)
        {
            _chatMessageServices = new ChatMessageServices(unitOfWork);
        }

        protected override IBaseManager<ChatMessageModel> GetManager()
        {
            return _chatMessageServices;
        }
    }
}
