using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Web_TiemHoaMuaXuan.Models.Entities;
using Web_TiemHoaMuaXuan.Services;

namespace Web_TiemHoaMuaXuan.Controllers
{
    public class CartController : Controller
    {
        private readonly OrderService _orderService = new OrderService();
        private readonly ProductService _productService = new ProductService();

        private List<CartItem> GetCart()
        {
            return Session["Cart"] as List<CartItem> ?? new List<CartItem>();
        }

        private void SaveCart(List<CartItem> cart)
        {
            Session["Cart"] = cart;
            Session["CartCount"] = cart.Sum(x => x.Quantity);
        }

        // GET: /Cart
        public ActionResult Index()
        {
            return View(GetCart());
        }

        // GET: /Cart/AddToCart/5
        public ActionResult AddToCart(int id, int quantity = 1)
        {
            var cart = GetCart();
            var p = _productService.GetById(id);

            if (p == null) { TempData["Error"] = "Sản phẩm không tồn tại."; return RedirectToAction("Index"); }
            if (p.Stock <= 0) { TempData["Error"] = "Sản phẩm này đã hết hàng."; return RedirectToAction("Index"); }

            var existing = cart.FirstOrDefault(x => x.ProductId == id);
            int currentQtyInCart = existing != null ? existing.Quantity : 0;

            if (currentQtyInCart + quantity > p.Stock)
            {
                TempData["Error"] = "Kho chỉ còn " + p.Stock + " sản phẩm. Bạn không thể thêm quá số lượng này.";
                return RedirectToAction("Index");
            }

            if (existing != null) { existing.Quantity += quantity; }
            else
            {
                cart.Add(new CartItem
                {
                    ProductId = p.Id,
                    ProductName = p.Name,
                    Image = p.ImageUrl,
                    Price = p.Price,
                    Quantity = quantity
                });
            }

            SaveCart(cart);
            TempData["Success"] = "Đã thêm vào giỏ hàng!";
            return RedirectToAction("Index");
        }

        // POST: /Cart/BuyNow
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult BuyNow(int id, int quantity = 1)
        {
            Session.Remove("Cart");
            Session.Remove("CartCount");
            Session.Remove("Coupon"); // Xóa mã cũ nếu có khi mua ngay

            var p = _productService.GetById(id);
            if (p == null || p.Stock <= 0 || quantity > p.Stock)
            {
                TempData["Error"] = "Sản phẩm không đủ số lượng hoặc đã hết hàng.";
                return RedirectToAction("Index", "Product");
            }

            var cart = new List<CartItem>();
            cart.Add(new CartItem
            {
                ProductId = p.Id,
                ProductName = p.Name,
                Image = p.ImageUrl,
                Price = p.Price,
                Quantity = quantity
            });

            SaveCart(cart);
            return RedirectToAction("Checkout");
        }

        // GET: /Cart/RemoveItem/5
        public ActionResult RemoveItem(int id)
        {
            var cart = GetCart();
            cart.RemoveAll(x => x.ProductId == id);
            SaveCart(cart);
            return RedirectToAction("Index");
        }

        // POST: /Cart/UpdateQuantity
        [HttpPost]
        public ActionResult UpdateQuantity(int id, int quantity)
        {
            var cart = GetCart();
            var item = cart.FirstOrDefault(x => x.ProductId == id);
            if (item != null && quantity > 0) item.Quantity = quantity;
            SaveCart(cart);
            return RedirectToAction("Index");
        }

        // GET: /Cart/Checkout
        public ActionResult Checkout()
        {
            var cart = GetCart();
            if (!cart.Any()) return RedirectToAction("Index", "Product");

            var user = Session["User"] as AppUser;
            if (user != null)
            {
                ViewBag.UserFullName = user.FullName;
                ViewBag.UserPhone = user.Phone;
                ViewBag.UserAddress = user.Address;
            }

            decimal total = cart.Sum(x => x.TotalPrice);
            ViewBag.TotalAmount = total;

            var appliedCoupon = Session["Coupon"] as Coupon;

            // ⭐ FIX 1: Kiểm tra lại xem giỏ hàng có bị giảm xuống dưới mức tối thiểu không
            if (appliedCoupon != null && total < appliedCoupon.MinOrderValue)
            {
                Session.Remove("Coupon"); // Hủy mã vì không đủ điều kiện
                appliedCoupon = null;
                TempData["Error"] = "Giỏ hàng của bạn đã thay đổi và không còn đủ điều kiện áp dụng mã giảm giá này.";
            }

            if (appliedCoupon != null)
            {
                ViewBag.Discount = appliedCoupon.DiscountAmount;
                // ⭐ FIX 2: Tránh hiển thị số tiền âm (nếu giảm giá > tổng tiền)
                decimal finalAmount = total - appliedCoupon.DiscountAmount;
                ViewBag.FinalAmount = finalAmount > 0 ? finalAmount : 0;
            }
            else
            {
                ViewBag.Discount = 0;
                ViewBag.FinalAmount = total;
            }

            ViewBag.AvailableCoupons = CouponData.Coupons
                .Where(c => c.IsActive && c.ExpiryDate > DateTime.Now)
                .ToList();

            var bankService = new BankSettingService();
            ViewBag.BankInfo = bankService.Get();

            return View();
        }

        // POST: /Cart/Checkout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Checkout(string fullName, string phone, string address, string payment)
        {
            var cart = GetCart();
            if (!cart.Any()) return RedirectToAction("Index", "Product");

            var user = Session["User"] as AppUser;
            string userEmail = user != null ? user.Email : "guest@tiemhoa.vn";

            // 1. Tính tổng tiền gốc
            decimal totalAmount = cart.Sum(x => x.TotalPrice);

            // ⭐ 2. TRỪ TIỀN GIẢM GIÁ NẾU CÓ MÃ (ĐÃ TÍCH HỢP VALIDATE CHỐNG HACK)
            var appliedCoupon = Session["Coupon"] as Coupon;
            if (appliedCoupon != null)
            {
                // FIX 3: Kiểm tra lại một lần nữa ở Server trước khi trừ tiền (chống F12 sửa HTML)
                if (totalAmount >= appliedCoupon.MinOrderValue && appliedCoupon.IsActive && appliedCoupon.ExpiryDate > DateTime.Now)
                {
                    totalAmount -= appliedCoupon.DiscountAmount;
                    if (totalAmount < 0) totalAmount = 0; // Tránh âm tiền
                }
                else
                {
                    // Nếu mã không hợp lệ, bỏ qua mã này, không trừ tiền
                    Session.Remove("Coupon");
                }
            }

            // 3. Tạo Order với tổng tiền ĐÃ GIẢM
            var order = new Order
            {
                UserEmail = userEmail,
                FullName = fullName,
                Phone = phone,
                Address = address,
                PaymentMethod = payment ?? "COD",
                Status = "Chờ xử lý",
                TotalAmount = totalAmount // ⭐ LƯU TỔNG TIỀN ĐÃ GIẢM
            };

            var items = cart.Select(x => new OrderItem
            {
                ProductId = x.ProductId,
                ProductName = x.ProductName,
                Image = string.IsNullOrWhiteSpace(x.Image) ? "/Content/images/1.jpg" : x.Image,
                Price = x.Price,
                Quantity = x.Quantity
            }).ToList();

            // 4. Lưu vào DB
            var savedOrder = _orderService.CreateOrder(order, items);

            // 5. Xóa giỏ và mã giảm giá
            Session.Remove("Cart");
            Session.Remove("CartCount");
            Session.Remove("Coupon");

            TempData["Success"] = "Đặt hàng thành công! Mã đơn: " + savedOrder.OrderCode;
            return RedirectToAction("OrderSuccess", new { id = savedOrder.Id });
        }

        // GET: /Cart/OrderSuccess/1
        public ActionResult OrderSuccess(int id)
        {
            var order = _orderService.GetById(id);
            if (order == null) return HttpNotFound();
            ViewBag.Title = "Đặt hàng thành công";
            return View(order);
        }

        // POST: /Cart/ApplyCoupon
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ApplyCoupon(string code)
        {
            var cart = GetCart();
            if (!cart.Any()) return Json(new { success = false, message = "Giỏ hàng trống." });

            decimal total = cart.Sum(x => x.TotalPrice);

            var validCoupon = CouponData.Coupons.FirstOrDefault(c =>
                c.Code.ToLower() == code.ToLower() &&
                c.IsActive &&
                c.ExpiryDate > DateTime.Now &&
                total >= c.MinOrderValue);

            if (validCoupon != null)
            {
                Session["Coupon"] = validCoupon;
                decimal discount = validCoupon.DiscountAmount;
                decimal finalAmount = total - discount;
                if (finalAmount < 0) finalAmount = 0;

                // Trả về JSON
                return Json(new
                {
                    success = true,
                    message = "Đã áp dụng mã giảm giá: -" + discount.ToString("N0") + "đ",
                    discount = discount,
                    finalAmount = finalAmount
                });
            }
            else
            {
                Session.Remove("Coupon");
                // Trả về JSON
                return Json(new
                {
                    success = false,
                    message = "Mã giảm giá không hợp lệ, hết hạn, hoặc đơn hàng chưa đủ điều kiện!"
                });
            }
        }
        // POST: /Cart/RemoveCoupon
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RemoveCoupon()
        {
            Session.Remove("Coupon");
            return RedirectToAction("Checkout");
        }
    }
}