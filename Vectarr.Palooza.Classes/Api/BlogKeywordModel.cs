using Vectarr.Palooza.Classes.BaseApi;

namespace Vectarr.Palooza.Classes.Api
{
    public class BlogKeywordModel : BaseApiModel
    {
        public int BlogId { get; set; }
        public string Keyword { get; set; } = null!;
    }
}
