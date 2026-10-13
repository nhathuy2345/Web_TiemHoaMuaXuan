using System.Linq;
using System.Web.Mvc;
using Web_TiemHoaMuaXuan.Models.Entities;
using Web_TiemHoaMuaXuan.Services;

namespace Web_TiemHoaMuaXuan.Controllers
{
    public class AdminController : Controller
    {
        private readonly ProductService _productService = new ProductService();
        private readonly OrderService _orderService = new OrderService();

        // =====================================================
        // DASHBOARD
        // =====================================================
        public ActionResult Dashboard()
        {
            ViewBag.Title = "Dashboard";
            ViewBag.TotalProducts = _productService.Count();
            ViewBag.TotalCategories = _productService.GetCategories().Count;
            ViewBag.TotalOrders = _orderService.Count();
            ViewBag.Revenue = _orderService.GetRevenue();
            return View();
        }

        // =====================================================
        // QUẢN LÝ SẢN PHẨM
        // =====================================================
        public ActionResult ProductManager()
        {
            ViewBag.Title = "Quản lý sản phẩm";
            var products = _productService.GetAll();
            return View(products);
        }

        // GET: /Admin/CreateProduct
        public ActionResult CreateProduct()
        {
            ViewBag.Title = "Thêm sản phẩm";
            ViewBag.Categories = new SelectList(_productService.GetCategories(), "Id", "Name");
            return View();
        }

        // POST: /Admin/CreateProduct
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateProduct(Product model)
        {
            if (string.IsNullOrWhiteSpace(model.Name) || model.Price <= 0)
            {
                ViewBag.Error = "Vui lòng nhập tên và giá sản phẩm";
                ViewBag.Categories = new SelectList(_productService.GetCategories(), "Id", "Name");
                return View(model);
            }

            if (string.IsNullOrWhiteSpace(model.ImageUrl))
                model.ImageUrl = "/Content/images/1.jpg";

            _productService.Add(model);

            TempData["Success"] = "Đã thêm sản phẩm: " + model.Name;
            return RedirectToAction("ProductManager");
        }

        // GET: /Admin/EditProduct/5
        public ActionResult EditProduct(int id)
        {
            var product = _productService.GetById(id);
            if (product == null) return HttpNotFound();

            ViewBag.Title = "Sửa sản phẩm";
            ViewBag.Categories = new SelectList(_productService.GetCategories(), "Id", "Name", product.CategoryId);
            return View(product);
        }

        // POST: /Admin/EditProduct
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditProduct(Product model)
        {
            var existing = _productService.GetById(model.Id);
            if (existing == null) return HttpNotFound();

            if (string.IsNullOrWhiteSpace(model.Name) || model.Price <= 0)
            {
                ViewBag.Error = "Vui lòng nhập tên và giá sản phẩm";
                ViewBag.Categories = new SelectList(_productService.GetCategories(), "Id", "Name", model.CategoryId);
                return View(model);
            }

            if (string.IsNullOrWhiteSpace(model.ImageUrl))
                model.ImageUrl = existing.ImageUrl;

            _productService.Update(model);

            TempData["Success"] = "Đã cập nhật sản phẩm: " + model.Name;
            return RedirectToAction("ProductManager");
        }

        // POST: /Admin/DeleteProduct/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteProduct(int id)
        {
            var product = _productService.GetById(id);
            if (product == null) return HttpNotFound();

            string name = product.Name;
            _productService.Delete(id);

            TempData["Success"] = "Đã xóa sản phẩm: " + name;
            return RedirectToAction("ProductManager");
        }

        // =====================================================
        // QUẢN LÝ ĐƠN HÀNG
        // =====================================================
        public ActionResult OrderManager()
        {
            ViewBag.Title = "Quản lý đơn hàng";
            var orders = _orderService.GetAll();
            return View(orders);
        }

        // GET: /Admin/OrderDetail/1
        public ActionResult OrderDetail(int id)
        {
            var order = _orderService.GetById(id);
            if (order == null) return HttpNotFound();

            ViewBag.Title = "Chi tiết đơn hàng #" + order.OrderCode;
            return View(order);
        }

        // POST: /Admin/UpdateOrderStatus
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateOrderStatus(int id, string status)
        {
            var validStatuses = new[] { "Chờ xử lý", "Đã xác nhận", "Đang giao hàng", "Đã giao", "Đã hủy" };
            if (!validStatuses.Contains(status))
            {
                TempData["Error"] = "Trạng thái không hợp lệ";
                return RedirectToAction("OrderDetail", new { id = id });
            }

            var order = _orderService.GetById(id);
            string oldStatus = order?.Status ?? "";

            if (_orderService.UpdateStatus(id, status))
            {
                TempData["Success"] = $"Đơn {order.OrderCode}: {oldStatus} → {status}";
            }
            return RedirectToAction("OrderDetail", new { id = id });
        }

        // Quick actions
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult QuickConfirm(int id)
        {
            var order = _orderService.GetById(id);
            if (order != null && order.Status == "Chờ xử lý")
            {
                _orderService.UpdateStatus(id, "Đã xác nhận");
                TempData["Success"] = "Đã xác nhận đơn " + order.OrderCode;
            }
            return RedirectToAction("OrderManager");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult QuickShip(int id)
        {
            var order = _orderService.GetById(id);
            if (order != null && order.Status == "Đã xác nhận")
            {
                _orderService.UpdateStatus(id, "Đang giao hàng");
                TempData["Success"] = "Đang giao đơn " + order.OrderCode;
            }
            return RedirectToAction("OrderManager");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult QuickDeliver(int id)
        {
            var order = _orderService.GetById(id);
            if (order != null && order.Status == "Đang giao hàng")
            {
                _orderService.UpdateStatus(id, "Đã giao");
                TempData["Success"] = "Đã giao đơn " + order.OrderCode;
            }
            return RedirectToAction("OrderManager");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult QuickCancel(int id)
        {
            var order = _orderService.GetById(id);
            if (order != null && order.Status != "Đã giao" && order.Status != "Đã hủy")
            {
                _orderService.UpdateStatus(id, "Đã hủy");
                TempData["Success"] = "Đã hủy đơn " + order.OrderCode;
            }
            else
            {
                TempData["Error"] = "Không thể hủy đơn này";
            }
            return RedirectToAction("OrderManager");
        }

        // =====================================================
        // CẤU HÌNH NGÂN HÀNG
        // =====================================================
        public ActionResult BankConfig()
        {
            ViewBag.Title = "Cấu hình ngân hàng";
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult BankConfig(string bankName, string accountNumber,
                                       string accountHolder, string qrImageUrl)
        {
            if (string.IsNullOrWhiteSpace(bankName) || string.IsNullOrWhiteSpace(accountNumber))
            {
                ViewBag.Error = "Vui lòng nhập tên ngân hàng và số tài khoản";
                return View();
            }

            BankSettings.BankName = bankName;
            BankSettings.AccountNumber = accountNumber;
            BankSettings.AccountHolder = accountHolder ?? "";
            BankSettings.QRImageUrl = qrImageUrl ?? "";

            TempData["Success"] = "Đã cập nhật cấu hình ngân hàng!";
            return RedirectToAction("BankConfig");
        }
    }
}