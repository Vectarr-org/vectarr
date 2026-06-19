using Vectarr.Palooza.Classes.BaseApi;

namespace Vectarr.Palooza.Classes.Api
{
    public class ApplicationModel : BaseApiModel
    {
        public int UserAccountId { get; set; }
        public int UserIdentityId { get; set; }
        public string UserEmail { get; set; }
        public int CallCredits { get; set; }
    }
}
