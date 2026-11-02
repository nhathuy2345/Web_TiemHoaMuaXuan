using System.Collections.Generic;
using System.Linq;
using Web_TiemHoaMuaXuan.Models;
using Web_TiemHoaMuaXuan.Models.Entities;

namespace Web_TiemHoaMuaXuan.Services
{
    public class ChatService
    {
        private readonly TiemHoaMuaXuanDbContext _db = new TiemHoaMuaXuanDbContext();

        public List<ChatMessage> GetMessages(string userEmail)
        {
            return _db.ChatMessages
                .Where(m => m.UserEmail == userEmail)
                .OrderBy(m => m.SentAt)
                .ToList();
        }

        public void SendMessage(string userEmail, string message, bool isAdmin)
        {
            _db.ChatMessages.Add(new ChatMessage
            {
                UserEmail = userEmail,
                Message = message,
                IsAdmin = isAdmin,
                SentAt = System.DateTime.Now
            });
            _db.SaveChanges();
        }

        public List<string> GetListUsersChat()
        {
            return _db.ChatMessages.Select(m => m.UserEmail).Distinct().ToList();
        }
    }
}