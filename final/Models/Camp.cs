using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace final.Models
{
    public class Camp
    {
        public int Id { get; set; }

        [Required]
        [StringLength(64)]
        public string Name { get; set; }

        [Required]
        [StringLength(255)]
        public string Description { get; set; }

        [Required]
        public double Latitude { get; set; }

        [Required]
        public double Longitude { get; set; }

        public string PhotoPath { get; set; }

        // Navigation property for reviews
        public virtual ICollection<Review> Reviews { get; set; }

        [NotMapped]
        public IFormFile PhotoUpload { get; set; }
    }
}
