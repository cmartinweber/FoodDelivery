using Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace FoodDelivery.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RewardPointsController : Controller
    {
        private readonly UnitOfWork _unitOfWork;
        public RewardPointsController(UnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        [HttpGet]
        public IActionResult Get()
        {
            var data = _unitOfWork.RewardPoint
                .List()
                .Select(r => new
                {
                    id = r.Id,
                    thresholdPoints = r.ThresholdPoints,
                    description = r.Description,
                    promoCode = r.PromoCode != null ? r.PromoCode.Code : "",
                    isActive = r.IsActive
                })
                .ToList();

            return Json(new { data });
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var objFromDb = _unitOfWork.RewardPoint.Get(r => r.Id == id);
            if (objFromDb == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }
            _unitOfWork.RewardPoint.Delete(objFromDb);
            _unitOfWork.Commit();
            return Json(new { success = true, message = "Delete successful" });
        }
    }
}
