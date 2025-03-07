using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.Models
{
    public class MenuItem
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [Display(Name="Menu Item")]
        public string Name { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        [Range(1, int.MaxValue, ErrorMessage ="Price should be greater than $1")]
        public float Price { get; set; }
        public int CategoryId { get; set; }
        public int FoodTypeID { get; set; }

        //Connect Objects or Tables
        [ForeignKey("CategoryId")]
        public virtual Category Category { get; set; }
        [ForeignKey("FoodTypeID")]
        public virtual FoodType FoodType { get; set; }

    }
}
