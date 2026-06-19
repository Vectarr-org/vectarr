using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vectarr.Palooza.Model
{
    public class BlogSection : BaseModel
    {
        public int BlogId { get; set; }
        public virtual Blog Blog { get; set; } = null!;

        public string? Heading { get; set; }               // null = intro paragraph (no heading)
        public string Content { get; set; } = null!;       // may contain inline HTML (<strong>, <a>, etc.)
        public int Order { get; set; }                     // display order within the article
    }
}
