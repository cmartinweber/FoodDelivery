using ApplicationCore.Models;
using Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FoodDelivery.Pages.Admin.Categories
{
    public class UpsertModel : PageModel
    {
        private readonly UnitOfWork _unitOfWork;

        [BindProperty]
        public Category CategoryObj { get; set; }

        public UpsertModel(UnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        public IActionResult OnGet(int ? id)
        {
            CategoryObj = new Category();
            if (id != 0)
            {//edit
                CategoryObj = _unitOfWork.Category.Get(u => u.ID == id);

                if (CategoryObj == null)
                {
                    return NotFound();
                }
            }
            return Page(); //assume insert new mode
        }

        public IActionResult OnPost()
        {
            if(!ModelState.IsValid)
            {
                return Page();
            }
            //If New
            if(CategoryObj.ID == 0)
            {
                _unitOfWork.Category.Add(CategoryObj);
            }
            //existing
            else
            {
                _unitOfWork.Category.Update(CategoryObj);
            }
            _unitOfWork.Commit();
            return RedirectToPage("./Index");
        }
    }
}
