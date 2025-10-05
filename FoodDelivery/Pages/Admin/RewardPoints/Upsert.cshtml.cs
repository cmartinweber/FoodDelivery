using ApplicationCore.Models;
using Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FoodDelivery.Pages.Admin.RewardPoints
{
    public class UpsertModel : PageModel
    {
        private readonly UnitOfWork _unitOfWork;

        [BindProperty]
        public RewardPoint RewardPointsObj { get; set; }

        public IEnumerable<SelectListItem> PromoCodeOptions { get; set; }

        public UpsertModel(UnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        public IActionResult OnGet(int? id)
        {
            RewardPointsObj = new RewardPoint();
            PromoCodeOptions = _unitOfWork.PromoCode.List()
                .Select(p => new SelectListItem
                {
                    Text = p.Code,
                    Value = p.Id.ToString()
                });

            if (id != 0 && id != null)
            {
                RewardPointsObj = _unitOfWork.RewardPoint.Get(u => u.Id == id);
                if (RewardPointsObj == null)
                {
                    return NotFound();
                }
            }
            return Page();
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                PromoCodeOptions = _unitOfWork.PromoCode.List()
                    .Select(p => new SelectListItem
                    {
                        Text = p.Code,
                        Value = p.Id.ToString()
                    });
                return Page();
            }

            if (RewardPointsObj.Id == 0)
            {
                _unitOfWork.RewardPoint.Add(RewardPointsObj);
            }
            else
            {
                _unitOfWork.RewardPoint.Update(RewardPointsObj);
            }

            _unitOfWork.Commit();
            return RedirectToPage("./Index");
        }
    }
}
