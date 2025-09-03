using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.Models
{
    public class MenuItemFoodType
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int MenuItemId { get; set; }

        [Required]
        public int FoodTypeId { get; set; }

        [ForeignKey("MenuItemId")]
        public virtual MenuItem MenuItem { get; set; }

        [ForeignKey("FoodTypeId")]
        public virtual FoodType FoodType { get; set; }
    }
}
