using ApplicationCore.Models;
using FoodDelivery.ViewModels;
using Infrastructure.Data;
using Infrastructure.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Stripe;
using System.Security.Claims;

namespace FoodDelivery.Pages.Customer.Cart
{
    public class SummaryModel : PageModel
    {
        private readonly UnitOfWork _unitOfWork;
        public SummaryModel(UnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        [BindProperty]
        public OrderDetailsCartVM OrderDetailsCart { get; set; }

        public void OnGet()
        {
            OrderDetailsCart = new OrderDetailsCartVM()
            {
                OrderHeader = new OrderHeader(),
                ListCart = new List<ShoppingCart>()
            };
            OrderDetailsCart.OrderHeader.OrderTotal = 0;
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if (claim != null)
            {
                IEnumerable<ShoppingCart> cart = _unitOfWork.ShoppingCart.List(c => c.ApplicationUserId == claim.Value);
                if (cart != null)
                {
                    OrderDetailsCart.ListCart = cart.ToList();
                }
                foreach (var cartList in OrderDetailsCart.ListCart)
                {
                    cartList.MenuItem = _unitOfWork.MenuItem.Get(m => m.Id == cartList.MenuItemId);
                    OrderDetailsCart.OrderHeader.OrderTotal += (cartList.MenuItem.Price * cartList.Count);
                }
                OrderDetailsCart.OrderHeader.OrderTotal += OrderDetailsCart.OrderHeader.OrderTotal * SD.SalesTaxPercent;
                ApplicationUser applicationUser = _unitOfWork.ApplicationUser.Get(c => c.Id == claim.Value);
                OrderDetailsCart.OrderHeader.DeliveryName = applicationUser.FullName;
                OrderDetailsCart.OrderHeader.PhoneNumber = applicationUser.PhoneNumber;
                OrderDetailsCart.OrderHeader.DeliveryTime = DateTime.Now;
                OrderDetailsCart.OrderHeader.DeliveryDate = DateTime.Now;
            }
        }

        public IActionResult OnPost(string stripeToken)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            OrderDetailsCart.ListCart = _unitOfWork.ShoppingCart.List(c => c.ApplicationUserId == claim.Value).ToList();
            OrderDetailsCart.OrderHeader.PaymentStatus = SD.PaymentStatusPending;
            OrderDetailsCart.OrderHeader.OrderDate = DateTime.Now;
            OrderDetailsCart.OrderHeader.UserId = claim.Value;
            OrderDetailsCart.OrderHeader.Status = SD.StatusSubmitted;
            OrderDetailsCart.OrderHeader.DeliveryTime = Convert.ToDateTime(OrderDetailsCart.OrderHeader.DeliveryDate.ToShortDateString() + " " + OrderDetailsCart.OrderHeader.DeliveryTime.ToShortTimeString());

            List<OrderDetails> orderDetailsList = new List<OrderDetails>();

            _unitOfWork.OrderHeader.Add(OrderDetailsCart.OrderHeader);
            _unitOfWork.Commit();

            foreach (var item in OrderDetailsCart.ListCart)
            {
                item.MenuItem = _unitOfWork.MenuItem.Get(m => m.Id == item.MenuItemId);
                OrderDetails orderDetails = new OrderDetails()
                {
                    MenuItemId = item.MenuItemId,
                    OrderId = OrderDetailsCart.OrderHeader.Id,
                    Name = item.MenuItem.Name,
                    Price = item.MenuItem.Price,
                    Count = item.Count
                };

                OrderDetailsCart.OrderHeader.OrderTotal += (orderDetails.Count * orderDetails.Price) * (1 + SD.SalesTaxPercent);
                _unitOfWork.OrderDetails.Add(orderDetails);
            }

            OrderDetailsCart.OrderHeader.OrderTotal = Convert.ToDouble(String.Format("{0:.##}", OrderDetailsCart.OrderHeader.OrderTotal));

            HttpContext.Session.SetInt32(SD.ShoppingCart, 0);
            _unitOfWork.Commit();

            //calling stripe pop up api through JS

            if (stripeToken != null)
            {
                var paymentIntentService = new PaymentIntentService();
                var paymentIntentOptions = new PaymentIntentCreateOptions
                {
                    Amount = Convert.ToInt32(OrderDetailsCart.OrderHeader.OrderTotal * 100), // Amount in cents
                    Currency = "usd",
                    PaymentMethod = stripeToken, // Use the token as the payment method
                    ConfirmationMethod = "manual",
                    Confirm = true,
                    ReturnUrl = Url.Page(
                        pageName: "/Customer/Cart/OrderConfirmation",
                        pageHandler: null, // Required placeholder
                        values: new { orderId = OrderDetailsCart.OrderHeader.Id },
                        protocol: Request.Scheme
                    )
                };

                var paymentIntent = paymentIntentService.Create(paymentIntentOptions);

                OrderDetailsCart.OrderHeader.TransactionId = paymentIntent.Id;

                if (paymentIntent.Status == "succeeded")
                {
                    OrderDetailsCart.OrderHeader.PaymentStatus = SD.PaymentStatusApproved;
                }
                else
                {
                    OrderDetailsCart.OrderHeader.PaymentStatus = SD.PaymentStatusRejected;
                }
            }

            _unitOfWork.Commit();

            var cartItems = _unitOfWork.ShoppingCart.List(c => c.ApplicationUserId == claim.Value).ToList();
            foreach (var cartItem in cartItems)
            {
                _unitOfWork.ShoppingCart.Delete(cartItem);
            }
            _unitOfWork.Commit();

            return RedirectToPage("/Customer/Cart/OrderConfirmation", new { orderId = OrderDetailsCart.OrderHeader.Id });
        }
    }
}
