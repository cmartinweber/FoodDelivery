using ApplicationCore.Models;
using Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FoodDelivery.Pages.Admin.PromoCodes
{
    public class UpsertModel : PageModel
    {
        private readonly UnitOfWork _unitOfWork;
        [BindProperty]
        public PromoCode PromoCodeObj { get; set; }
        public UpsertModel(UnitOfWork unitOfWork) => _unitOfWork = unitOfWork;
        public IActionResult OnGet(int? id)
        {
            PromoCodeObj = new PromoCode();
            if (id != 0 && id != null)
            {//edit
                PromoCodeObj = _unitOfWork.PromoCode.Get(u => u.Id == id);
                if (PromoCodeObj == null)
                {
                    return NotFound();
                }
            }
            return Page(); //assume insert new mode
        }
        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            //If New
            if (PromoCodeObj.Id == 0)
            {
                _unitOfWork.PromoCode.Add(PromoCodeObj);
            }
            //existing
            else
            {
                _unitOfWork.PromoCode.Update(PromoCodeObj);
            }
            _unitOfWork.Commit();
            return RedirectToPage("./Index");
        }
    }
}
