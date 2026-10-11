using System.Collections.Generic;
using System.Linq;
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
            if (!GetCart().Any()) return RedirectToAction("Index", "Product");
            return View();
        }

        // POST: /Cart/Checkout
        [HttpPost]
        public ActionResult Checkout(string fullName, string phone, string address, string payment)
        {
            Session.Remove("Cart");
            Session.Remove("CartCount");
            TempData["Success"] = "Đặt hàng thành công! Cảm ơn " + fullName;
            return RedirectToAction("Index", "Home");
        }
    }
}