using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.Models
{
    public class Category
    {
        [Key]
        public int ID { get; set; }

        [Required]
        [Display(Name = "Category Name")]
        public string? Name { get; set; }

        [Display(Name = "Display Order")]
        [Range(1, 100, ErrorMessage = "You must enter 1 to 100")]
        public int DisplayOrder {  get; set; }
    }
}
