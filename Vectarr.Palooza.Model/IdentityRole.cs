using Vectarr.Palooza.CoreClasses.Enums;
using Microsoft.AspNetCore.Identity;

namespace Vectarr.Palooza.Model
{
    public class IdentityRole : IdentityRole<int>, IBaseModel
    {
        public IdentityRole()
        {
            CreatedDate = DateTime.UtcNow;
            LastModified = DateTime.UtcNow;
        }

        public DateTime LastModified { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsRemoved { get; set; }
        public bool IsHidden { get; set; }
        public RolesEnum Role { get; set; }
    }
}
