using System.Web.Mvc;
using Web_TiemHoaMuaXuan.Models.Entities;
using Web_TiemHoaMuaXuan.Services;

namespace Web_TiemHoaMuaXuan.Controllers
{
    public class ChatController : Controller
    {
        private readonly ChatService _chatService = new ChatService();

        // API: Lấy tin nhắn của User
        [HttpGet]
        public JsonResult GetMessages(string email)
        {
            var user = Session["User"] as AppUser;
            string userEmail = email ?? (user != null ? user.Email : "guest@springflower.com");

            var messages = _chatService.GetMessages(userEmail);
            return Json(messages, JsonRequestBehavior.AllowGet);
        }

        // API: Gửi tin nhắn
        [HttpPost]
        public JsonResult SendMessage(string message, bool isAdmin = false, string targetEmail = null)
        {
            var user = Session["User"] as AppUser;

            // Nếu là Admin trả lời -> gửi cho targetEmail
            // Nếu là Khách gửi -> lấy email khách
            string userEmail = isAdmin ? targetEmail : (user != null ? user.Email : "guest@springflower.com");

            if (string.IsNullOrWhiteSpace(message) || string.IsNullOrWhiteSpace(userEmail))
                return Json(new { success = false });

            _chatService.SendMessage(userEmail, message, isAdmin);
            return Json(new { success = true });
        }
    }
}