using System.Linq;
using System.Web.Mvc;
using Web_TiemHoaMuaXuan.Models.Entities;

namespace Web_TiemHoaMuaXuan.Controllers
{
    public class AdminController : Controller
    {
        // =====================================================
        // DASHBOARD
        // =====================================================
        public ActionResult Dashboard()
        {
            ViewBag.Title = "Dashboard";
            ViewBag.TotalProducts = MockData.Products.Count;
            ViewBag.TotalCategories = MockData.Categories.Count;
            ViewBag.TotalOrders = MockData.Orders.Count;
            ViewBag.Revenue = MockData.Orders
                .Where(o => o.Status == "Đã giao")
                .Sum(o => o.TotalAmount);
            return View();
        }

        // =====================================================
        // QUẢN LÝ SẢN PHẨM - INDEX
        // =====================================================
        public ActionResult ProductManager()
        {
            ViewBag.Title = "Quản lý sản phẩm";
            var products = MockData.Products.OrderBy(p => p.Id).ToList();
            return View(products);
        }

        // =====================================================
        // THÊM SẢN PHẨM (Create)
        // =====================================================
        // GET: /Admin/CreateProduct
        public ActionResult CreateProduct()
        {
            ViewBag.Title = "Thêm sản phẩm";
            ViewBag.Categories = new SelectList(MockData.Categories, "Id", "Name");
            return View();
        }

        // POST: /Admin/CreateProduct
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateProduct(Product model)
        {
            // Validate cơ bản
            if (string.IsNullOrWhiteSpace(model.Name) || model.Price <= 0)
            {
                ViewBag.Error = "Vui lòng nhập tên và giá sản phẩm";
                ViewBag.Categories = new SelectList(MockData.Categories, "Id", "Name");
                return View(model);
            }

            // Tạo SP mới với Id tự sinh
            var newProduct = new Product
            {
                Id = MockData.NextProductId(),
                Name = model.Name,
                Description = model.Description ?? "",
                Price = model.Price,
                OldPrice = model.OldPrice,
                DiscountPercent = model.DiscountPercent,
                ImageUrl = string.IsNullOrWhiteSpace(model.ImageUrl)
                    ? "/Content/images/1.jpg"
                    : model.ImageUrl,
                Stock = model.Stock,
                CategoryId = model.CategoryId,
                IsFeatured = model.IsFeatured
            };

            MockData.Products.Add(newProduct);

            TempData["Success"] = "Đã thêm sản phẩm: " + newProduct.Name;
            return RedirectToAction("ProductManager");
        }

        // =====================================================
        // SỬA SẢN PHẨM (Edit)
        // =====================================================
        // GET: /Admin/EditProduct/5
        public ActionResult EditProduct(int id)
        {
            var product = MockData.GetProduct(id);
            if (product == null) return HttpNotFound();

            ViewBag.Title = "Sửa sản phẩm";
            ViewBag.Categories = new SelectList(MockData.Categories, "Id", "Name", product.CategoryId);
            return View(product);
        }

        // POST: /Admin/EditProduct/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditProduct(Product model)
        {
            var existing = MockData.GetProduct(model.Id);
            if (existing == null) return HttpNotFound();

            // Validate
            if (string.IsNullOrWhiteSpace(model.Name) || model.Price <= 0)
            {
                ViewBag.Error = "Vui lòng nhập tên và giá sản phẩm";
                ViewBag.Categories = new SelectList(MockData.Categories, "Id", "Name", model.CategoryId);
                return View(model);
            }

            // Cập nhật
            existing.Name = model.Name;
            existing.Description = model.Description ?? "";
            existing.Price = model.Price;
            existing.OldPrice = model.OldPrice;
            existing.DiscountPercent = model.DiscountPercent;
            if (!string.IsNullOrWhiteSpace(model.ImageUrl))
                existing.ImageUrl = model.ImageUrl;
            existing.Stock = model.Stock;
            existing.CategoryId = model.CategoryId;
            existing.IsFeatured = model.IsFeatured;

            TempData["Success"] = "Đã cập nhật sản phẩm: " + existing.Name;
            return RedirectToAction("ProductManager");
        }

        // =====================================================
        // XÓA SẢN PHẨM (Delete)
        // =====================================================
        // POST: /Admin/DeleteProduct/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteProduct(int id)
        {
            var product = MockData.GetProduct(id);
            if (product == null) return HttpNotFound();

            string name = product.Name;
            MockData.Products.Remove(product);

            TempData["Success"] = "Đã xóa sản phẩm: " + name;
            return RedirectToAction("ProductManager");
        }

        // =====================================================
        // QUẢN LÝ ĐƠN HÀNG
        // =====================================================
        public ActionResult OrderManager()
        {
            ViewBag.Title = "Quản lý đơn hàng";
            var orders = MockData.Orders.OrderByDescending(o => o.OrderDate).ToList();
            return View(orders);
        }

        // GET: /Admin/OrderDetail/1
        public ActionResult OrderDetail(int id)
        {
            var order = MockData.GetOrder(id);
            if (order == null) return HttpNotFound();

            ViewBag.Title = "Chi tiết đơn hàng #" + order.OrderCode;
            return View(order);
        }

        // =====================================================
        // CẬP NHẬT TRẠNG THÁI ĐƠN HÀNG
        // =====================================================
        // POST: /Admin/UpdateOrderStatus
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateOrderStatus(int id, string status)
        {
            var order = MockData.GetOrder(id);
            if (order == null) return HttpNotFound();

            // Validate trạng thái hợp lệ
            var validStatuses = new[] { "Chờ xử lý", "Đã xác nhận", "Đang giao hàng", "Đã giao", "Đã hủy" };
            if (!validStatuses.Contains(status))
            {
                TempData["Error"] = "Trạng thái không hợp lệ";
                return RedirectToAction("OrderDetail", new { id = id });
            }

            string oldStatus = order.Status;
            order.Status = status;

            TempData["Success"] = $"Đơn {order.OrderCode}: {oldStatus} → {status}";
            return RedirectToAction("OrderDetail", new { id = id });
        }

        // =====================================================
        // XÁC NHẬN NHANH (từ danh sách đơn)
        // =====================================================
        // POST: /Admin/QuickConfirm/1
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult QuickConfirm(int id)
        {
            var order = MockData.GetOrder(id);
            if (order == null) return HttpNotFound();

            if (order.Status == "Chờ xử lý")
            {
                order.Status = "Đã xác nhận";
                TempData["Success"] = "Đã xác nhận đơn " + order.OrderCode;
            }
            return RedirectToAction("OrderManager");
        }

        // POST: /Admin/QuickShip/1
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult QuickShip(int id)
        {
            var order = MockData.GetOrder(id);
            if (order == null) return HttpNotFound();

            if (order.Status == "Đã xác nhận")
            {
                order.Status = "Đang giao hàng";
                TempData["Success"] = "Đang giao đơn " + order.OrderCode;
            }
            return RedirectToAction("OrderManager");
        }

        // POST: /Admin/QuickDeliver/1
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult QuickDeliver(int id)
        {
            var order = MockData.GetOrder(id);
            if (order == null) return HttpNotFound();

            if (order.Status == "Đang giao hàng")
            {
                order.Status = "Đã giao";
                TempData["Success"] = "Đã giao đơn " + order.OrderCode;
            }
            return RedirectToAction("OrderManager");
        }

        // POST: /Admin/QuickCancel/1
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult QuickCancel(int id)
        {
            var order = MockData.GetOrder(id);
            if (order == null) return HttpNotFound();

            if (order.Status != "Đã giao" && order.Status != "Đã hủy")
            {
                order.Status = "Đã hủy";
                TempData["Success"] = "Đã hủy đơn " + order.OrderCode;
            }
            else
            {
                TempData["Error"] = "Không thể hủy đơn đã giao hoặc đã hủy";
            }
            return RedirectToAction("OrderManager");
        }
        // =====================================================
        // CẤU HÌNH NGÂN HÀNG (Bank Config)
        // =====================================================
        // GET: /Admin/BankConfig
        public ActionResult BankConfig()
        {
            ViewBag.Title = "Cấu hình ngân hàng";
            return View();
        }

        // POST: /Admin/BankConfig
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult BankConfig(string bankName, string accountNumber,
                                       string accountHolder, string qrImageUrl)
        {
            if (string.IsNullOrWhiteSpace(bankName) ||
                string.IsNullOrWhiteSpace(accountNumber))
            {
                ViewBag.Error = "Vui lòng nhập tên ngân hàng và số tài khoản";
                return View();
            }


            // THÀNH:
            BankSettings.BankName = bankName;
            BankSettings.AccountNumber = accountNumber;
            BankSettings.AccountHolder = accountHolder ?? "";
            BankSettings.QRImageUrl = qrImageUrl ?? "";

            TempData["Success"] = "Đã cập nhật cấu hình ngân hàng!";
            return RedirectToAction("BankConfig");
        }
    }
}