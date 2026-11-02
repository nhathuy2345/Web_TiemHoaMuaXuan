using System.Linq;
using Web_TiemHoaMuaXuan.Models;
using Web_TiemHoaMuaXuan.Models.Entities;

namespace Web_TiemHoaMuaXuan.Services
{
    /// <summary>
    /// Service quản lý cấu hình ngân hàng
    /// Chỉ có 1 row (Id = 1), tự tạo nếu chưa có
    /// </summary>
    public class BankSettingService
    {
        private readonly TiemHoaMuaXuanDbContext _db = new TiemHoaMuaXuanDbContext();

        /// <summary>
        /// Lấy config hiện tại. Nếu chưa có → tạo mặc định
        /// </summary>
        public BankSetting Get()
        {
            var setting = _db.BankSettings.FirstOrDefault();
            if (setting == null)
            {
                setting = new BankSetting
                {
                    BankName = "Vietcombank",
                    AccountNumber = "0123456789",
                    AccountHolder = "TIEM HOA MUA XUAN",
                    QRImageUrl = ""
                };
                _db.BankSettings.Add(setting);
                _db.SaveChanges();
            }
            return setting;
        }

        /// <summary>
        /// Cập nhật config
        /// </summary>
        public void Update(BankSetting model)
        {
            var setting = _db.BankSettings.FirstOrDefault();
            if (setting == null)
            {
                model.Id = 0;
                _db.BankSettings.Add(model);
            }
            else
            {
                setting.BankName = model.BankName;
                setting.AccountNumber = model.AccountNumber;
                setting.AccountHolder = model.AccountHolder;
                setting.QRImageUrl = model.QRImageUrl;
            }
            _db.SaveChanges();
        }
    }
}