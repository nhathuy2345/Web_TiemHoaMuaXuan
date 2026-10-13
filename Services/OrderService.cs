using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using Web_TiemHoaMuaXuan.Models;
using Web_TiemHoaMuaXuan.Models.Entities;

namespace Web_TiemHoaMuaXuan.Services
{
    /// <summary>
    /// Service xử lý Order — đọc/ghi DB
    /// </summary>
    public class OrderService
    {
        private readonly TiemHoaMuaXuanDbContext _db = new TiemHoaMuaXuanDbContext();

        // Lấy tất cả đơn (mới nhất trước)
        public List<Order> GetAll()
        {
            return _db.Orders
                .Include("Items")
                .OrderByDescending(o => o.OrderDate)
                .ToList();
        }

        // Lấy đơn theo Id (có include Items)
        public Order GetById(int id)
        {
            return _db.Orders
                .Include("Items")
                .FirstOrDefault(o => o.Id == id);
        }

        // Lấy đơn của 1 user theo email
        public List<Order> GetByEmail(string email)
        {
            return _db.Orders
                .Include("Items")
                .Where(o => o.UserEmail == email)
                .OrderByDescending(o => o.OrderDate)
                .ToList();
        }

        // Đếm tổng đơn
        public int Count()
        {
            return _db.Orders.Count();
        }

        // Tính tổng doanh thu (chỉ tính đơn đã giao)
        public decimal GetRevenue()
        {
            return _db.Orders
                .Where(o => o.Status == "Đã giao")
                .Select(o => o.TotalAmount)
                .DefaultIfEmpty(0)
                .Sum();
        }

        /// <summary>
        /// Tạo đơn hàng mới — lưu cả Order + OrderItems
        /// </summary>
        public Order CreateOrder(Order order, List<OrderItem> items)
        {
            // Sinh OrderCode tự động dựa trên số đơn hiện có
            int nextId = _db.Orders.Any() ? _db.Orders.Max(o => o.Id) + 1 : 1;
            order.OrderCode = "DH" + (1000 + nextId);
            order.OrderDate = DateTime.Now;

            _db.Orders.Add(order);
            _db.SaveChanges();   // Lưu Order trước để có Id

            // Gán OrderId cho từng Item rồi lưu
            foreach (var item in items)
            {
                item.OrderId = order.Id;
                _db.OrderItems.Add(item);
            }
            _db.SaveChanges();

            return order;
        }

        // Cập nhật trạng thái đơn
        public bool UpdateStatus(int orderId, string status)
        {
            var order = _db.Orders.Find(orderId);
            if (order == null) return false;

            order.Status = status;
            _db.SaveChanges();
            return true;
        }
    }
}