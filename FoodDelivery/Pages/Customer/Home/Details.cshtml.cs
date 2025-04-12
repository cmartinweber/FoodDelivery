using ApplicationCore.Models;
using Infrastructure.Data;
using Infrastructure.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace FoodDelivery.Pages.Customer.Home
{
    public class DetailsModel : PageModel
    {
        private readonly UnitOfWork _unitOfWork;

        [BindProperty]
        public int txtCount { get; set; } = 1;

        public MenuItem objMenuItem { get; set; }
        public ShoppingCart objCart { get; set; }

        public DetailsModel(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult OnGet(int? id)
        {
            if (id != null)
            {
                //check to see if user logged in
                var claimsIdentity = User.Identity as ClaimsIdentity;
                var claim = claimsIdentity?.FindFirst(ClaimTypes.NameIdentifier);
                HttpContext.Session.SetString("UserLoggedIn", claim?.Value ?? string.Empty);

                objMenuItem = _unitOfWork.MenuItem.Get(m => m.Id == id, false, "Category,FoodType");
                objCart = new ShoppingCart { MenuItemId = id.Value };
            }
            else
            {
                throw new Exception("Menu Item Not Found");
            }
            return Page();
        }

        public IActionResult OnPost(MenuItem objMenuItem)
        {
            var claimsIdentity = User.Identity as ClaimsIdentity;
            var userId = claimsIdentity?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var existingCart = _unitOfWork.ShoppingCart.Get(u => u.ApplicationUserId == userId && u.MenuItemId == objMenuItem.Id);

            if (existingCart == null)
            {
                var newCart = new ShoppingCart
                {
                    ApplicationUserId = userId,
                    MenuItemId = objMenuItem.Id,
                    Count = txtCount
                };
                _unitOfWork.ShoppingCart.Add(newCart);
            }
            else
            {
                existingCart.Count += txtCount;
                _unitOfWork.ShoppingCart.Update(existingCart);
            }

            _unitOfWork.Commit();

            var updatedCart = _unitOfWork.ShoppingCart.List(u => u.ApplicationUserId == userId).ToList();
            HttpContext.Session.SetInt32(SD.ShoppingCart, updatedCart.Count());

            return RedirectToPage("Index");
        }
    }
}
