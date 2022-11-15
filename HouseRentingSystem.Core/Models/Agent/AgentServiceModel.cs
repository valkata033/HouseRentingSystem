using System.ComponentModel.DataAnnotations;

namespace HouseRentingSystem.Core.Models.Agent
{
    public class AgentServiceModel
    {
        [Required]
        [StringLength(15, MinimumLength = 7)]
        [Display(Name = "Phone Number")]
        [Phone]
        public string PhoneNumber { get; set; } = null!;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

    }
}
