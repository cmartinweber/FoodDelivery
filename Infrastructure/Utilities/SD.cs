using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Utilities
{
    public class SD
    {
        public const string AdminRole = "Admin";
        public const string DriverRole = "Driver";
        public const string CustomerRole = "Customer";
        public const string KitchenRole = "Staff";

        public const string ShoppingCart = "ShoppingCart";

        public const float SalesTaxPercent = 0.0825f;
        public const float SalesTaxRate = 8.25f;

        public const string PaymentStatusPending = "Payment Pending";
        public const string PaymentStatusApproved = "Payment Approved";
        public const string PaymentStatusRejected = "Payment Rejected";
        public const string StatusSubmitted = "Order Submitted";
        public const string StatusReady = "Order Ready";
        public const string StatusDelivered = "Order Delivered";
        public const string StatusCancelled = "Order Cancelled";
        public const string StatusRefunded = "Order Refunded";

    }
}
