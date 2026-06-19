using Vectarr.Palooza.Classes.Api;
using Vectarr.Palooza.DataAccess.IInfrastructure;
using Vectarr.Palooza.Model;

namespace Vectarr.Palooza.DataAccess.IRepositories
{
    public interface IChatMessageRepository : IEntityRepository<ChatMessage, ChatMessageModel>
    {
    }
}
