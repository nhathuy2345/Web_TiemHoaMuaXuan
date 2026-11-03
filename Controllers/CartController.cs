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

        // Cần thêm DbContext để hàm CheckPaymentStatus có thể truy vấn DB
        private readonly Web_TiemHoaMuaXuan.Models.TiemHoaMuaXuanDbContext db = new Web_TiemHoaMuaXuan.Models.TiemHoaMuaXuanDbContext();

        private List<CartItem> GetCart()
        {
            return Session["Cart"] as List<CartItem> ?? new List<CartItem>();
        }

        private void SaveCart(List<CartItem> cart)
        {
            Session["Cart"] = cart;
            Session["CartCount"] = cart.Sum(x => x.Quantity);
        }

        // ... (Giữ nguyên các hàm Index, AddToCart, BuyNow, RemoveItem, UpdateQuantity như cũ) ...

        public ActionResult Index()
        {
            return View(GetCart());
        }

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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult BuyNow(int id, int quantity = 1)
        {
            Session.Remove("Cart");
            Session.Remove("CartCount");
            Session.Remove("Coupon");

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

        public ActionResult RemoveItem(int id)
        {
            var cart = GetCart();
            cart.RemoveAll(x => x.ProductId == id);
            SaveCart(cart);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult UpdateQuantity(int id, int quantity)
        {
            var cart = GetCart();
            var item = cart.FirstOrDefault(x => x.ProductId == id);

            if (item != null && quantity > 0)
            {
                var product = _productService.GetById(id);
                if (product != null && quantity > product.Stock)
                {
                    item.Quantity = product.Stock;
                    TempData["Error"] = $"Sản phẩm '{product.Name}' chỉ còn tối đa {product.Stock} sản phẩm trong kho.";
                }
                else
                {
                    item.Quantity = quantity;
                }
            }
            SaveCart(cart);
            return RedirectToAction("Index");
        }

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
            if (appliedCoupon != null && total < appliedCoupon.MinOrderValue)
            {
                Session.Remove("Coupon");
                appliedCoupon = null;
                TempData["Error"] = "Giỏ hàng của bạn đã thay đổi và không còn đủ điều kiện áp dụng mã giảm giá này.";
            }

            if (appliedCoupon != null)
            {
                ViewBag.Discount = appliedCoupon.DiscountAmount;
                decimal finalAmount = total - appliedCoupon.DiscountAmount;
                ViewBag.FinalAmount = finalAmount > 0 ? finalAmount : 0;
            }
            else
            {
                ViewBag.Discount = 0;
                ViewBag.FinalAmount = total;
            }

            ViewBag.AvailableCoupons = CouponData.Coupons.Where(c => c.IsActive && c.ExpiryDate > DateTime.Now).ToList();

            var bankService = new BankSettingService();
            ViewBag.BankInfo = bankService.Get();
            // ⭐ THÊM 2 DÒNG NÀY VÀO ĐÂY ⭐
            // Tạo mã đơn hàng tạm thời (VD: DH202611031530)
            string tempOrderCode = "DH" + DateTime.Now.ToString("yyyyMMddHHmmss");
            ViewBag.TempOrderCode = tempOrderCode;

            return View();
        }

        // POST: /Cart/Checkout (Hàm xử lý đặt hàng)
        [HttpPost]
        [ValidateAntiForgeryToken]
        // ⭐ 1. THÊM THAM SỐ string orderCode VÀO ĐÂY
        public ActionResult Checkout(string fullName, string phone, string address, string payment, string orderCode)
        {
            var cart = GetCart();
            if (!cart.Any()) return RedirectToAction("Index", "Product");

            // KIỂM TRA DỮ LIỆU ĐẦU VÀO (CHỐNG F12 BYPASS)
            if (string.IsNullOrWhiteSpace(fullName) || string.IsNullOrWhiteSpace(phone) || string.IsNullOrWhiteSpace(address))
            {
                TempData["Error"] = "Vui lòng nhập đầy đủ thông tin giao hàng (Tên, SĐT, Địa chỉ).";
                return RedirectToAction("Checkout");
            }

            // Kiểm tra định dạng số điện thoại (10-11 số)
            if (!System.Text.RegularExpressions.Regex.IsMatch(phone, @"^\d{10,11}$"))
            {
                TempData["Error"] = "Số điện thoại không hợp lệ (Phải là 10 hoặc 11 chữ số).";
                return RedirectToAction("Checkout");
            }

            var user = Session["User"] as AppUser;
            string userEmail = user != null ? user.Email : "guest@tiemhoa.vn";

            decimal totalAmount = cart.Sum(x => x.TotalPrice);

            // KIỂM TRA TỒN KHO
            foreach (var item in cart)
            {
                var product = _productService.GetById(item.ProductId);
                if (product == null || product.Stock < item.Quantity)
                {
                    string pName = product != null ? product.Name : "Sản phẩm";
                    int currentStock = product != null ? product.Stock : 0;
                    TempData["Error"] = $"Rất tiếc! Sản phẩm '{pName}' chỉ còn {currentStock} trong kho. Vui lòng cập nhật lại giỏ hàng.";
                    return RedirectToAction("Index");
                }
            }

            // TRỪ TIỀN GIẢM GIÁ
            var appliedCoupon = Session["Coupon"] as Coupon;
            if (appliedCoupon != null)
            {
                if (totalAmount >= appliedCoupon.MinOrderValue && appliedCoupon.IsActive && appliedCoupon.ExpiryDate > DateTime.Now)
                {
                    totalAmount -= appliedCoupon.DiscountAmount;
                    if (totalAmount < 0) totalAmount = 0;
                }
                else
                {
                    Session.Remove("Coupon");
                }
            }

            // 4. Tạo Order
            var order = new Order
            {
                // ⭐ 2. GÁN MÃ ĐƠN HÀNG TỪ FORM GỬI LÊN
                OrderCode = string.IsNullOrEmpty(orderCode) ? "DH" + DateTime.Now.ToString("yyyyMMddHHmmss") : orderCode,
                UserEmail = userEmail,
                FullName = fullName,
                Phone = phone,
                Address = address,
                PaymentMethod = payment ?? "COD",
                Status = (payment == "Bank") ? "Chờ thanh toán" : "Chờ xử lý",
                TotalAmount = totalAmount
            };

            var items = cart.Select(x => new CartItem
            {
                ProductId = x.ProductId,
                ProductName = x.ProductName,
                Image = string.IsNullOrWhiteSpace(x.Image) ? "/Content/images/1.jpg" : x.Image,
                Price = x.Price,
                Quantity = x.Quantity
            }).Select(x => new OrderItem
            {
                ProductId = x.ProductId,
                ProductName = x.ProductName,
                Image = x.Image,
                Price = x.Price,
                Quantity = x.Quantity
            }).ToList();

            // 5. Lưu vào DB
            var savedOrder = _orderService.CreateOrder(order, items);

            // 6. Xóa giỏ hàng
            Session.Remove("Cart");
            Session.Remove("CartCount");
            Session.Remove("Coupon");

            TempData["Success"] = "Đặt hàng thành công! Mã đơn: " + savedOrder.OrderCode;
            return RedirectToAction("OrderSuccess", new { id = savedOrder.Id });
        }
        public ActionResult OrderSuccess(int id)
        {
            var order = _orderService.GetById(id);
            if (order == null) return HttpNotFound();

            ViewBag.Title = "Đặt hàng thành công";

            // Nếu thanh toán bằng ngân hàng, tạo link QR và truyền sang View
            if (order.PaymentMethod == "Bank")
            {
                ViewBag.QrUrl = GenerateSePayQrUrl(order.TotalAmount, order.OrderCode);
            }

            return View(order);
        }

        // Hàm hỗ trợ tạo link ảnh QR của SePay
        private string GenerateSePayQrUrl(decimal amount, string orderCode)
        {
            // ⭐ THAY ĐỔI THÔNG TIN NÀY BẰNG TÀI KHOẢN CỦA BẠN
            string accountNumber = "79128462666"; // Số tài khoản TPBank bạn đã liên kết
            string bankCode = "TPBank";           // Mã ngân hàng
            string description = "Thanh toan don hang " + orderCode; // Nội dung chuyển khoản

            // Nối chuỗi tạo URL theo chuẩn của SePay
            return $"https://qr.sepay.vn/img?acc={accountNumber}&bank={bankCode}&amount={(int)amount}&des={Uri.EscapeDataString(description)}";
        }
        

        // ⭐ HÀM MỚI: Dành cho Javascript gọi để kiểm tra trạng thái thanh toán
        [HttpGet]
        public JsonResult CheckPaymentStatus(string orderCode)
        {
            var order = db.Orders.FirstOrDefault(o => o.OrderCode == orderCode);
            if (order == null) return Json(new { status = "not_found" }, JsonRequestBehavior.AllowGet);

            // Nếu trạng thái là "Đã thanh toán" (do Webhook cập nhật), trả về "paid"
            string status = (order.Status == "Đã thanh toán") ? "paid" : "pending";
            return Json(new { status = status }, JsonRequestBehavior.AllowGet);
        }

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
                return Json(new
                {
                    success = false,
                    message = "Mã giảm giá không hợp lệ, hết hạn, hoặc đơn hàng chưa đủ điều kiện!"
                });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RemoveCoupon()
        {
            Session.Remove("Coupon");
            return RedirectToAction("Checkout");
        }
    }
}