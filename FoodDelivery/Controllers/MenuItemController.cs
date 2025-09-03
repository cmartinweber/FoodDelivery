using ApplicationCore.Interfaces;
using Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FoodDelivery.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MenuItemController : Controller
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _hostingEnv;
        public MenuItemController(UnitOfWork unitOfWork, IWebHostEnvironment hostingEnv)
        {
            _unitOfWork = unitOfWork;
            _hostingEnv = hostingEnv;
        }
        [HttpGet]
        public IActionResult Get()
        {
            var menuItems = _unitOfWork.MenuItem.List(
                null,
                null,
                "Category,MenuItemFoodTypes.FoodType"
            );
            var data = menuItems.Select(m => new
            {
                m.Id,
                m.Name,
                m.Price,
                Category = new { m.Category.ID, m.Category.Name },
                MenuItemFoodTypes = m.MenuItemFoodTypes.Select(ft => new
                {
                    ft.FoodTypeId,
                    FoodType = new { ft.FoodType.Id, ft.FoodType.Name }
                }),
                m.Image
            });

            return Json(new { data });
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var objFromDb = _unitOfWork.MenuItem.Get(c => c.Id == id, true, "MenuItemFoodTypes");
            if (objFromDb == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }
            if(objFromDb.Image != null)
            {
                var imgPath = Path.Combine(_hostingEnv.WebRootPath, objFromDb.Image.TrimStart('\\'));
                if (System.IO.File.Exists(imgPath))//image physically there
                {
                    System.IO.File.Delete(imgPath);
                }
            }
            if(objFromDb.MenuItemFoodTypes != null && objFromDb.MenuItemFoodTypes.Any())
            {
                _unitOfWork.MenuItemFoodType.Delete(objFromDb.MenuItemFoodTypes);
            }
            _unitOfWork.MenuItem.Delete(objFromDb);
            _unitOfWork.Commit();
            return Json(new { success = true, message = "Delete successful" });
        }
    }
}
