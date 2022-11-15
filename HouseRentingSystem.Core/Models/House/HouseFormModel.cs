using System.ComponentModel.DataAnnotations;

namespace HouseRentingSystem.Core.Models.House
{
    public class HouseFormModel
    {
        [Required]
        [StringLength(50, MinimumLength = 10)]
        public string Title { get; set; } = null!;

        [Required]
        [StringLength(150, MinimumLength = 30)]
        public string Address { get; set; } = null!;

        [Required]
        [StringLength(500, MinimumLength = 50)]
        public string Description { get; set; } = null!;

        [Required]
        [Display(Name = "Image Url")]
        [StringLength(200, MinimumLength = 0)]
        public string ImageUrl { get; set; } = null!;

        [Required]
        [Display(Name = "Price per month")]
        [Range(0.00, 2000.0, ErrorMessage = "Price per month must be a positive number and less then 2000 leva.")]
        public decimal PricePerMonth { get; set; }

        [Required]
        [Display(Name = "Category")]
        public int CategoryId { get; set; }

        public IEnumerable<HouseCategoryServiceModel> Categories { get; set; }
            = new List<HouseCategoryServiceModel>();
    }
}
