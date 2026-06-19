using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Vectarr.Palooza.Model
{
    [Table("Referral.Referral")]
    public class Referral : BaseModel
    {
        public virtual UserAccount ReferredUser { get; set; }
        public int ReferredUserId { get; set; }
        public bool FreeTubEarned { get; set; }
        public Application Application { get; set; }
    }
}
