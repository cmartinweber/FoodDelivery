using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApplicationCore.Models
{
    public class RewardPoint
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "Points Required")]
        public int ThresholdPoints { get; set; }

        [Display(Name = "Reward Description")]
        public string? Description { get; set; }

        public int? PromoCodeId { get; set; }

        [ForeignKey("PromoCodeId")]
        public PromoCode? PromoCode { get; set; }

        [Display(Name = "Is Active")]
        public bool IsActive { get; set; } = true;
    }
}
