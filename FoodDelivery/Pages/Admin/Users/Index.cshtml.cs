using ApplicationCore.Models;
using Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FoodDelivery.Pages.Admin.Users
{
    public class IndexModel : PageModel
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly UserManager<IdentityUser> _userManager;

        public IndexModel(UnitOfWork unitOfWork, UserManager<IdentityUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        public IEnumerable<ApplicationUser> ApplicationUsers { get; set; }
        public Dictionary<string, List<string>> UserRoles { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; }

        public async Task OnGetAsync(bool success = false, string message = null)
        {
            Success = success;
            Message = message;
            UserRoles = new Dictionary<string, List<string>>();
            ApplicationUsers = _unitOfWork.ApplicationUser.List();
            foreach(var user in ApplicationUsers)
            {
                var userRole = await _userManager.GetRolesAsync(user);
                UserRoles.Add(user.Id, userRole.ToList());
            }
        }

        public async Task<IActionResult> OnPostLockUnlock(string id)
        {
            var user = _unitOfWork.ApplicationUser.Get(u => u.Id == id);
            if(user.LockoutEnd == null)//unlocked
            {
                user.LockoutEnd =DateTime.Now.AddYears(100);
                user.LockoutEnabled = true;
            }
            else if (user.LockoutEnd > DateTime.Now) //unlock
            {
                user.LockoutEnd = DateTime.Now;
                user.LockoutEnabled = false;
            }
            else
            {
                user.LockoutEnd = DateTime.Now.AddYears(100);
                user.LockoutEnabled = true;
            }
            _unitOfWork.ApplicationUser.Update(user);
            await _unitOfWork.CommitAsync();
            return RedirectToPage();
        }
    }
}
