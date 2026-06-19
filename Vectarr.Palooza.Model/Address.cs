using Vectarr.Palooza.CoreClasses.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Vectarr.Palooza.Model
{
    [Table("Address.Addresses")]
    public class Address : BaseModel
    {
        public CountryEnum? Country { get; set; }
        public string? Street1 { get; set; }
        public string? Street2 { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? PostalCode { get; set; }
    }
}
