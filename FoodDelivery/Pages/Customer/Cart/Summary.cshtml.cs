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

        [BindProperty(SupportsGet = true)]
        public string PromoCodeInput { get; set; }

        [BindProperty(SupportsGet = true)]
        public string PromoMessage { get; set; }

        [BindProperty(SupportsGet = true)]
        public bool PromoApplied { get; set; }

        [BindProperty(SupportsGet = true)]
        public double DiscountAmount { get; set; }

        public void OnGet()
        {
            LoadOrderDetails();
        }

        public IActionResult OnPostApplyPromo()
        {
            LoadOrderDetails();

            var orderTotal = OrderDetailsCart.OrderHeader.OrderTotal;
            var promo = _unitOfWork.PromoCode.Get(p => p.Code == PromoCodeInput);

            if (promo == null)
                return RedirectToPage(new { PromoApplied = false, DiscountAmount = 0d, PromoMessage = "Invalid Promo Code", PromoCodeInput });

            if (!promo.IsActive)
                return RedirectToPage(new { PromoApplied = false, DiscountAmount = 0d, PromoMessage = "Promo Code is not active", PromoCodeInput });

            if (promo.ValidTo.HasValue && promo.ValidTo < DateTime.Now)
                return RedirectToPage(new { PromoApplied = false, DiscountAmount = 0d, PromoMessage = "Promo Code has expired", PromoCodeInput });

            if (promo.ValidFrom.HasValue && promo.ValidFrom > DateTime.Now)
                return RedirectToPage(new { PromoApplied = false, DiscountAmount = 0d, PromoMessage = "Promo Code is not valid yet", PromoCodeInput });

            if (promo.UsesRemaining.HasValue && promo.UsesRemaining <= 0)
                return RedirectToPage(new { PromoApplied = false, DiscountAmount = 0d, PromoMessage = "Promo Code has been used up", PromoCodeInput });

            if (promo.MinimumOrderValue.HasValue && orderTotal < promo.MinimumOrderValue)
                return RedirectToPage(new { PromoApplied = false, DiscountAmount = 0d, PromoMessage = $"Minimum order value for this promo code is {promo.MinimumOrderValue:C}", PromoCodeInput });

            var discount = promo.DiscountType == DiscountType.FixedAmount
                ? (double)promo.DiscountValue
                : orderTotal * (double)(promo.DiscountValue / 100);

            if (promo.UsesRemaining.HasValue && promo.UsesRemaining > 0)
            {
                promo.UsesRemaining -= 1;
                _unitOfWork.PromoCode.Update(promo);
                _unitOfWork.Commit();
            }

            return RedirectToPage(new
            {
                PromoApplied = true,
                DiscountAmount = discount,
                PromoMessage = $"Promo code applied. You saved {discount:C}",
                PromoCodeInput = promo.Code
            });
        }

        public IActionResult OnPostRemovePromo()
        {
            return RedirectToPage(new
            {
                PromoApplied = false,
                DiscountAmount = 0d,
                PromoMessage = "",
                PromoCodeInput = ""
            });
        }

        public IActionResult OnPost(string stripeToken)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            LoadOrderDetails();

            OrderDetailsCart.ListCart = _unitOfWork.ShoppingCart.List(c => c.ApplicationUserId == claim.Value).ToList();
            OrderDetailsCart.OrderHeader.PaymentStatus = SD.PaymentStatusPending;
            OrderDetailsCart.OrderHeader.OrderDate = DateTime.Now;
            OrderDetailsCart.OrderHeader.UserId = claim.Value;
            OrderDetailsCart.OrderHeader.Status = SD.StatusSubmitted;
            OrderDetailsCart.OrderHeader.DeliveryTime = Convert.ToDateTime(
                OrderDetailsCart.OrderHeader.DeliveryDate.ToShortDateString() + " " +
                OrderDetailsCart.OrderHeader.DeliveryTime.ToShortTimeString());

            if (PromoApplied && DiscountAmount <= 0 && !string.IsNullOrWhiteSpace(PromoCodeInput))
            {
                var subtotalWithTax = OrderDetailsCart.ListCart.Sum(i => i.MenuItem.Price * i.Count) * (1 + SD.SalesTaxPercent);
                var promo = _unitOfWork.PromoCode.Get(p => p.Code == PromoCodeInput);
                if (promo != null &&
                    promo.IsActive &&
                    (!promo.ValidFrom.HasValue || promo.ValidFrom <= DateTime.Now) &&
                    (!promo.ValidTo.HasValue || promo.ValidTo >= DateTime.Now) &&
                    (!promo.MinimumOrderValue.HasValue || subtotalWithTax >= promo.MinimumOrderValue))
                {
                    DiscountAmount = promo.DiscountType == DiscountType.FixedAmount
                        ? (double)promo.DiscountValue
                        : subtotalWithTax * (double)promo.DiscountValue;
                }
            }

            OrderDetailsCart.OrderHeader.DiscountAmount = DiscountAmount;
            OrderDetailsCart.OrderHeader.PromoCode = string.IsNullOrWhiteSpace(PromoCodeInput) ? null : PromoCodeInput;

            _unitOfWork.OrderHeader.Add(OrderDetailsCart.OrderHeader);
            _unitOfWork.Commit();

            foreach (var item in OrderDetailsCart.ListCart)
            {
                item.MenuItem = _unitOfWork.MenuItem.Get(m => m.Id == item.MenuItemId);
                var orderDetails = new OrderDetails
                {
                    MenuItemId = item.MenuItemId,
                    OrderId = OrderDetailsCart.OrderHeader.Id,
                    Name = item.MenuItem.Name,
                    Price = item.MenuItem.Price,
                    Count = item.Count
                };
                _unitOfWork.OrderDetails.Add(orderDetails);
            }

            OrderDetailsCart.OrderHeader.OrderTotal =
                OrderDetailsCart.ListCart.Sum(i => i.MenuItem.Price * i.Count) * (1 + SD.SalesTaxPercent)
                - OrderDetailsCart.OrderHeader.DiscountAmount;

            if (OrderDetailsCart.OrderHeader.OrderTotal < 0)
                OrderDetailsCart.OrderHeader.OrderTotal = 0;

            OrderDetailsCart.OrderHeader.OrderTotal =
                Convert.ToDouble(string.Format("{0:.##}", OrderDetailsCart.OrderHeader.OrderTotal));

            HttpContext.Session.SetInt32(SD.ShoppingCart, 0);
            _unitOfWork.Commit();

            if (stripeToken != null)
            {
                var paymentIntentService = new PaymentIntentService();
                var paymentIntentOptions = new PaymentIntentCreateOptions
                {
                    Amount = Convert.ToInt32(OrderDetailsCart.OrderHeader.OrderTotal * 100),
                    Currency = "usd",
                    PaymentMethod = stripeToken,
                    ConfirmationMethod = "manual",
                    Confirm = true,
                    ReturnUrl = Url.Page(
                        pageName: "/Customer/Cart/OrderConfirmation",
                        pageHandler: null,
                        values: new { orderId = OrderDetailsCart.OrderHeader.Id },
                        protocol: Request.Scheme
                    )
                };

                var paymentIntent = paymentIntentService.Create(paymentIntentOptions);

                OrderDetailsCart.OrderHeader.TransactionId = paymentIntent.Id;
                OrderDetailsCart.OrderHeader.PaymentStatus =
                    paymentIntent.Status == "succeeded" ? SD.PaymentStatusApproved : SD.PaymentStatusRejected;
            }

            _unitOfWork.Commit();

            var cartItems = _unitOfWork.ShoppingCart.List(c => c.ApplicationUserId == claim.Value).ToList();
            foreach (var cartItem in cartItems)
                _unitOfWork.ShoppingCart.Delete(cartItem);
            _unitOfWork.Commit();

            return RedirectToPage("/Customer/Cart/OrderConfirmation", new { orderId = OrderDetailsCart.OrderHeader.Id });
        }

        private void LoadOrderDetails()
        {
            if (OrderDetailsCart == null)
                OrderDetailsCart = new OrderDetailsCartVM();

            if (OrderDetailsCart.OrderHeader == null)
                OrderDetailsCart.OrderHeader = new OrderHeader();

            if (OrderDetailsCart.ListCart == null)
                OrderDetailsCart.ListCart = new List<ShoppingCart>();

            OrderDetailsCart.OrderHeader.OrderTotal = 0;

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity?.FindFirst(ClaimTypes.NameIdentifier);
            if (claim == null) return;

            var cart = _unitOfWork.ShoppingCart.List(c => c.ApplicationUserId == claim.Value);
            if (cart != null)
                OrderDetailsCart.ListCart = cart.ToList();

            foreach (var c in OrderDetailsCart.ListCart)
            {
                c.MenuItem = _unitOfWork.MenuItem.Get(m => m.Id == c.MenuItemId);
                OrderDetailsCart.OrderHeader.OrderTotal += (c.MenuItem.Price * c.Count);
            }

            OrderDetailsCart.OrderHeader.OrderTotal +=
                OrderDetailsCart.OrderHeader.OrderTotal * SD.SalesTaxPercent;

            var applicationUser = _unitOfWork.ApplicationUser.Get(c => c.Id == claim.Value);
            OrderDetailsCart.OrderHeader.DeliveryName = applicationUser?.FullName;
            OrderDetailsCart.OrderHeader.PhoneNumber = applicationUser?.PhoneNumber;
            OrderDetailsCart.OrderHeader.DeliveryTime = DateTime.Now;
            OrderDetailsCart.OrderHeader.DeliveryDate = DateTime.Now;
        }
    }
}
