using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.Models
{
    public class RewardUsage
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string ApplicationUserId { get; set; }
        [ForeignKey("ApplicationUserId")]
        public ApplicationUser ApplicationUser { get; set; }
        [Required]
        public int RewardPointsId { get; set; }
        [ForeignKey("RewardPointsId")]
        public RewardPoint RewardPoints { get; set; }
        public DateTime GrantedOn { get; set; } = DateTime.UtcNow;
        public bool Notified { get; set; } = false;
        public bool Redeemed { get; set; } = false;
        public DateTime? RedeemedOn { get; set; }

    }
}
