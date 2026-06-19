using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vectarr.Palooza.Model
{
    [Table("Creator.Creators")]
    public class Creator : BaseModel
    {
        public int UserAccountId { get; set; }
        public virtual UserAccount UserAccount { get; set; }
        public string? Bio { get; set; }
        public Image? ProfilePicture { get; set; }
        public string? Company { get; set; }
        public int? AddressId { get; set; } // nullable
        public virtual Address? Address { get; set; } // nullable
        public virtual ICollection<Job> Jobs { get; set; }
        public virtual ICollection<WorkOrder> WorkOrders { get; set; }
    }
}
