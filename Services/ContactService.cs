using System;
using System.Collections.Generic;
using System.Linq;
using Web_TiemHoaMuaXuan.Models;
using Web_TiemHoaMuaXuan.Models.Entities;

namespace Web_TiemHoaMuaXuan.Services
{
    /// <summary>
    /// Service xử lý tin nhắn liên hệ
    /// </summary>
    public class ContactService
    {
        private readonly TiemHoaMuaXuanDbContext _db = new TiemHoaMuaXuanDbContext();

        public List<ContactMessage> GetAll()
        {
            return _db.ContactMessages
                .OrderByDescending(m => m.SentAt)
                .ToList();
        }

        public ContactMessage GetById(int id)
        {
            return _db.ContactMessages.FirstOrDefault(m => m.Id == id);
        }

        public void Add(ContactMessage message)
        {
            message.SentAt = DateTime.Now;
            message.IsRead = false;
            _db.ContactMessages.Add(message);
            _db.SaveChanges();
        }

        public void MarkAsRead(int id)
        {
            var msg = _db.ContactMessages.Find(id);
            if (msg != null)
            {
                msg.IsRead = true;
                _db.SaveChanges();
            }
        }

        public void Delete(int id)
        {
            var msg = _db.ContactMessages.Find(id);
            if (msg != null)
            {
                _db.ContactMessages.Remove(msg);
                _db.SaveChanges();
            }
        }

        public int CountUnread()
        {
            return _db.ContactMessages.Count(m => !m.IsRead);
        }
    }
}