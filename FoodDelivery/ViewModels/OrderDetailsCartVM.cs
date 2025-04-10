using ApplicationCore.Models;

namespace FoodDelivery.ViewModels
{
    public class OrderDetailsCartVM
    {
        public OrderHeader OrderHeader { get; set; }

        public List<ShoppingCart> ListCart { get; set; }
    }
}
