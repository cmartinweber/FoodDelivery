using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.Models
{
    public class FoodType
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Food Type Name")]
        public string Name { get; set; }

        public virtual ICollection<MenuItemFoodType> MenuItemFoodTypes { get; set; } = new List<MenuItemFoodType>();
    }
}
