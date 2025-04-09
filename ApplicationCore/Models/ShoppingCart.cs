using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.Models
{
    public class ShoppingCart
    {
        public int Id { get; set; }

        [Range(1, 100, ErrorMessage = "Count must be between 1 and 100.")]
        public int Count { get; set; }
        public int MenuItemId { get; set; }
        [NotMapped]
        public virtual MenuItem MenuItem { get; set; }
        public string ApplicationUserId { get; set; }
        [NotMapped]
        public virtual ApplicationUser ApplicationUser { get; set; }

    }
}
