using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace final.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [StringLength(50)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(50)]
        public string LastName { get; set; }

        // Flag to identify admin users
        public bool IsAdmin { get; set; } = false;

        // Navigation property for user reviews
        public virtual ICollection<Review> Reviews { get; set; }
    }
}
