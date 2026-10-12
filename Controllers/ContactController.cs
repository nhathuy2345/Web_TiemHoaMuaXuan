using System.Web.Mvc;

namespace Web_TiemHoaMuaXuan.Controllers
{
    public class ContactController : Controller
    {
        // GET: /Contact
        public ActionResult Index()
        {
            ViewBag.Title = "Liên hệ";
            return View();
        }

        // POST: /Contact
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(string name, string email, string phone, string message)
        {
            // Validate
            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(message))
            {
                ViewBag.Error = "Vui lòng nhập tên và nội dung tin nhắn";
                return View();
            }

            // Fake lưu tin nhắn
            TempData["Success"] = "Cảm ơn " + name + "! Chúng tôi sẽ liên hệ với bạn trong 24h.";
            return RedirectToAction("Index");
        }
    }
}