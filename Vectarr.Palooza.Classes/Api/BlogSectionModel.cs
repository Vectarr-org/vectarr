using Vectarr.Palooza.Classes.BaseApi;

namespace Vectarr.Palooza.Classes.Api
{
    public class BlogSectionModel : BaseApiModel
    {
        public int BlogId { get; set; }
        public string? Heading { get; set; }
        public string Content { get; set; } = null!;
        public int Order { get; set; }
    }
}
