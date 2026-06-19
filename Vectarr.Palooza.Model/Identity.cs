using System.ComponentModel.DataAnnotations.Schema;
using Vectarr.Palooza.CoreClasses.Enums;

namespace Vectarr.Palooza.Model
{
    [Table("Identity.Identities")]
    public class Identity : BaseModel
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string EmailAddress { get; set; }
        public virtual Address? Address { get; set; }
        public virtual Image? ProfilePicture { get; set; }
        public GenderEnum? Gender { get; set; }
        public string? PhoneNumber { get; set; }
        public int? Age { get; set; }
        public bool IsEmailSubscribed { get; set; }
        public decimal? TaxRate { get; set; }
    }
}
