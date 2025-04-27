namespace final.Models
{
    public class CampingSiteViewModel
    {
        public Camp Camp { get; set; }
        public Review NewReview { get; set; }
        public IEnumerable<Review> Reviews { get; set; }
    }
}
