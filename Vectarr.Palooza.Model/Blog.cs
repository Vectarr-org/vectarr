using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vectarr.Palooza.Model
{
    public class Blog : BaseModel
    {
        public string Slug { get; set; } = null!;          // URL-friendly identifier
        public string Title { get; set; } = null!;
        public string Excerpt { get; set; } = null!;        // Short summary for listing cards
        public string Description { get; set; } = null!;   // SEO meta description (full article)
        public virtual Image Image { get; set; }
        public int ImageId { get; set; }                    // e.g. "assets/blog/cnc-guide.svg" or CDN URL
        public string Category { get; set; } = null!;
        public int ReadTimeMinutes { get; set; }            // stored as int, rendered as "10 min read"
        public DateTime PublishedAt { get; set; }

        public virtual ICollection<BlogSection> Sections { get; set; } = [];
        public virtual ICollection<BlogKeyword> Keywords { get; set; } = [];
    }
}
