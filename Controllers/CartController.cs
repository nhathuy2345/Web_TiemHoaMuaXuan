using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web_TiemHoaMuaXuan.Models;
using Web_TiemHoaMuaXuan.Models.Entities;

namespace Web_TiemHoaMuaXuan.Controllers
{
    public class CartController : Controller
    {
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
            var existing = cart.FirstOrDefault(x => x.ProductId == id);

            if (existing != null)
            {
                existing.Quantity += quantity;
            }
            else
            {
                var p = MockData.GetProduct(id);
                if (p != null)
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
            }

            SaveCart(cart);
            TempData["Success"] = "Đã thêm vào giỏ hàng!";
            return RedirectToAction("Index");
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
            if (!cart.Any())
                return RedirectToAction("Index", "Product");

            // Nếu đã login → prefill thông tin
            var user = Session["User"] as AppUser;
            if (user != null)
            {
                ViewBag.UserFullName = user.FullName;
                ViewBag.UserPhone = user.Phone;
                ViewBag.UserAddress = user.Address;
            }

            ViewBag.TotalAmount = cart.Sum(x => x.TotalPrice);
            return View();
        }

        // POST: /Cart/Checkout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Checkout(string fullName, string phone, string address, string payment)
        {
            var cart = GetCart();
            if (!cart.Any())
                return RedirectToAction("Index", "Product");

            // Lấy email user (nếu đã login) hoặc "guest"
            var user = Session["User"] as AppUser;
            string userEmail = user != null ? user.Email : "guest@tiemhoa.vn";

            // ===== TẠO ĐƠN HÀNG MỚI =====
            var order = new Order
            {
                Id = MockData.Orders.Count > 0 ? MockData.Orders.Max(o => o.Id) + 1 : 1,
                OrderCode = MockData.NextOrderCode(),
                UserEmail = userEmail,
                FullName = fullName,
                Phone = phone,
                Address = address,
                PaymentMethod = payment ?? "COD",
                Status = "Chờ xử lý",
                OrderDate = DateTime.Now,
                TotalAmount = cart.Sum(x => x.TotalPrice),
                Items = cart.Select(x => new OrderItem
                {
                    ProductId = x.ProductId,
                    ProductName = x.ProductName,
                    Image = x.Image,
                    Price = x.Price,
                    Quantity = x.Quantity
                }).ToList()
            };

            // Lưu vào MockData
            MockData.Orders.Add(order);

            // Xóa giỏ hàng
            Session.Remove("Cart");
            Session.Remove("CartCount");

            TempData["Success"] = "Đặt hàng thành công! Mã đơn: " + order.OrderCode;
            return RedirectToAction("OrderSuccess", new { id = order.Id });
        }

        // GET: /Cart/OrderSuccess/1
        public ActionResult OrderSuccess(int id)
        {
            var order = MockData.GetOrder(id);
            if (order == null) return HttpNotFound();

            ViewBag.Title = "Đặt hàng thành công";
            return View(order);
        }
    }
}