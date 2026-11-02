using System.Web.Mvc;
using Web_TiemHoaMuaXuan.Models.Entities;
using Web_TiemHoaMuaXuan.Services;

namespace Web_TiemHoaMuaXuan.Controllers
{
    public class ContactController : Controller
    {
        private readonly ContactService _contactService = new ContactService();

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
            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(message))
            {
                ViewBag.Error = "Vui lòng nhập tên và nội dung tin nhắn";
                return View();
            }

            var newMessage = new ContactMessage
            {
                Name = name,
                Email = email,
                Phone = phone ?? "",
                Message = message
            };

            _contactService.Add(newMessage);

            TempData["Success"] = "Cảm ơn " + name + "! Chúng tôi sẽ liên hệ với bạn trong 24h.";
            return RedirectToAction("Index");
        }
    }
}