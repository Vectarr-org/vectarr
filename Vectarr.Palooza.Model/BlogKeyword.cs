using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vectarr.Palooza.Model
{
    public class BlogKeyword : BaseModel
    {
        public int BlogId { get; set; }
        public virtual Blog Blog { get; set; } = null!;

        public string Keyword { get; set; } = null!;       // SEO keyword
    }
}
