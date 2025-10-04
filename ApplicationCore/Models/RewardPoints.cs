using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApplicationCore.Models
{
    public class RewardPoints
    {
        [Key]
        public int Id { get; set; }

        public string ApplicationUserId { get; set; }

        [ForeignKey("ApplicationUserId")]
        public ApplicationUser ApplicationUser { get; set; }

        [Display(Name = "Points Required")]
        public int ThresholdPoints { get; set; }

        [Display(Name = "Discount Amount ($)")]
        public decimal DiscountAmount { get; set; }

        [Display(Name = "Reward Description")]
        public string? Description { get; set; }

        public int? PromoCodeId { get; set; }

        [ForeignKey("PromoCodeId")]
        public PromoCode? PromoCode { get; set; }

        [Display(Name = "Is Active")]
        public bool IsActive { get; set; } = true;

        [Display(Name = "Is Used")]
        public bool IsUsed { get; set; } = false;
    }
}
