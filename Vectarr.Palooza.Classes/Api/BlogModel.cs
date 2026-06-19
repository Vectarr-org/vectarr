using Vectarr.Palooza.Classes.BaseApi;

namespace Vectarr.Palooza.Classes.Api
{
    public class BlogModel : BaseApiModel
    {
        public string Slug { get; set; } = null!;
        public string Title { get; set; } = null!;
        public string Excerpt { get; set; } = null!;
        public string Description { get; set; } = null!;
        public ImageModel? Image { get; set; }
        public int ImageId { get; set; }
        public string Category { get; set; } = null!;
        public int ReadTimeMinutes { get; set; }
        public DateTime PublishedAt { get; set; }
        public List<BlogSectionModel> Sections { get; set; } = [];
        public List<BlogKeywordModel> Keywords { get; set; } = [];
    }
}
