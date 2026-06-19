using System.ComponentModel.DataAnnotations.Schema;

namespace Vectarr.Palooza.Model
{
    [Table("Application.Applications")]
    public class Application : BaseModel
    {
        public virtual UserAccount User { get; set; }
        public int UserId { get; set; }
        public virtual ICollection<ApplicationSubscription> Subscriptions { get; set; }
        public bool IsVectarr { get; set; }
    }
}
