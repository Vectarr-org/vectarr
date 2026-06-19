using System.ComponentModel.DataAnnotations;
using Vectarr.Palooza.CoreClasses.Enums;

namespace Vectarr.Palooza.Classes.Api
{
    public class RegistrationModel
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string EmailAddress { get; set; }
        public string Password { get; set; }
        public string? ConfirmPassword { get; set; }
        public bool IsExternal { get; set; }
        public CreatorModel? Creator { get; set; }
        public WorkshopModel? Workshop { get; set; }
        public bool IsCreator { get; set; }
        public bool IsWorkshop { get; set; }
        public JobModel? Job { get; set; }
    }
}
