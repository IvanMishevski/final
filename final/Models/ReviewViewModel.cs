using System.ComponentModel.DataAnnotations;

namespace final.Models
{
    public class ReviewViewModel
    {
        [Required]
        public int CampId { get; set; }

        [Required]
        [StringLength(1000, MinimumLength = 10, ErrorMessage = "Review content must be between 10 and 1000 characters.")]
        public string Content { get; set; }

        [Required]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5.")]
        public int Rating { get; set; }
    }
}
