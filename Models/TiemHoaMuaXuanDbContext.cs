using System.Data.Entity;
using Web_TiemHoaMuaXuan.Models.Entities;

namespace Web_TiemHoaMuaXuan.Models
{
    /// <summary>
    /// DbContext — Cầu nối giữa code C# và SQL Server
    /// Mỗi DbSet tương ứng 1 bảng trong DB
    /// </summary>
    public class TiemHoaMuaXuanDbContext : DbContext
    {
        public TiemHoaMuaXuanDbContext() : base("name=TiemHoaConnection")
        {
            // Tắt lazy loading để tránh lỗi vòng lặp
            Configuration.LazyLoadingEnabled = false;
            Configuration.ProxyCreationEnabled = false;
        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<ContactMessage> ContactMessages { get; set; }
        public DbSet<BankSetting> BankSettings { get; set; }
    }
}