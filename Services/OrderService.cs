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
        /// Tạo đơn hàng mới — lưu cả Order + OrderItems trong 1 transaction
        /// </summary>
        public Order CreateOrder(Order order, List<OrderItem> items)
        {
            using (var tx = _db.Database.BeginTransaction())
            {
                _db.Orders.Add(order);
                _db.SaveChanges(); // Lưu Order trước để lấy Id duy nhất từ DB

                // Sinh OrderCode từ Id thật của DB để đảm bảo duy nhất, không trùng lặp
                order.OrderCode = "DH" + (1000 + order.Id);

                // Lưu từng Item và trừ tồn kho
                foreach (var item in items)
                {
                    item.OrderId = order.Id;
                    _db.OrderItems.Add(item);

                    var product = _db.Products.Find(item.ProductId);
                    if (product != null)
                    {
                        product.Stock -= item.Quantity;
                        if (product.Stock < 0) product.Stock = 0; // Đảm bảo kho không bị âm
                    }
                }

                _db.SaveChanges(); // Lưu OrderCode, OrderItems và cập nhật Stock
                tx.Commit();
            }

            return order;
        }
        public void RestoreStock(int orderId)
        {
            var order = _db.Orders.Include("Items").FirstOrDefault(o => o.Id == orderId);
            if (order != null)
            {
                foreach (var item in order.Items)
                {
                    var product = _db.Products.Find(item.ProductId);
                    if (product != null)
                    {
                        product.Stock += item.Quantity;
                    }
                }
                _db.SaveChanges();
            }
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