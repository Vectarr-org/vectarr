using System.ComponentModel.DataAnnotations;


namespace Vectarr.Palooza.Classes.Email
{
    public class NotificationEmailModel
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the email.
        /// </summary>
        /// <value>The email.</value>
        [Required]
        [EmailAddress(ErrorMessage = "invalid email address.")]
        public string Email { get; set; }
    }
}

