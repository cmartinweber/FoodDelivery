using ApplicationCore.Models;
using Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FoodDelivery.Pages.Customer.Home
{
    public class IndexModel : PageModel
    {
        public readonly UnitOfWork _unitOfWork;
        public IndexModel(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public List<MenuItem> MenuItemList { get; set; }
        public List<Category> CategoryList { get; set; }

        public void OnGet()
        {
            MenuItemList = _unitOfWork.MenuItem.List(null, null, "Category,MenuItemFoodTypes.FoodType").ToList();
            CategoryList = _unitOfWork.Category.List(null, c => c.DisplayOrder, null).ToList();
        }
    }
}
