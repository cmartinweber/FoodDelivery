using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.Models
{
    public enum DiscountType
    {
        Percentage,
        FixedAmount
    }
    public class PromoCode
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Promo Code")]
        public string Code { get; set; }

        public String? Description { get; set; }

        [Required]
        [Display(Name = "Discount Type")]
        public DiscountType DiscountType { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Discount value must be greater than 0")]
        [Display(Name = "Discount Value")]
        public double DiscountValue { get; set; }

        [Display(Name = "Valid From")]
        public DateTime? ValidFrom { get; set; }

        [Display(Name = "Valid To")]
        public DateTime? ValidTo { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Minimum order value must be non-negative")]
        [Display(Name = "Minimum Order Value")]
        public double? MinimumOrderValue { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Uses remaining must be non-negative")]
        [Display(Name = "Uses Remaining")]
        public int? UsesRemaining { get; set; }

        [Required]
        [Display(Name = "Is Active")]
        public bool IsActive { get; set; }

    }
}
