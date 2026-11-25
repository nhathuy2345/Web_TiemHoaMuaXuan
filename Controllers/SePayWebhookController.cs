using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using Newtonsoft.Json.Linq; // Bắt buộc phải có thư viện này
using Web_TiemHoaMuaXuan.Models; // Namespace chứa DbContext của bạn

namespace Web_TiemHoaMuaXuan.Controllers
{
    public class SePayWebhookController : Controller
    {
        // Khai báo DbContext để làm việc với Database
        private readonly TiemHoaMuaXuanDbContext db = new TiemHoaMuaXuanDbContext();

        [HttpPost]
        [AllowAnonymous] // Cho phép SePay gọi vào mà không cần đăng nhập
        public ActionResult Index()
        {
            try
            {
                // 1. XÁC THỰC API KEY
                string apiKey = Request.Headers["Authorization"];

                // Lấy API Key đã lưu trong Database (do Admin nhập trên web)
                var bankService = new Web_TiemHoaMuaXuan.Services.BankSettingService();
                var bankConfig = bankService.Get();

                // Nếu trong DB có lưu API Key thì dùng, không thì dùng mặc định
                string expectedKey = (bankConfig != null && !string.IsNullOrEmpty(bankConfig.SePayApiKey))
                                     ? "Apikey " + bankConfig.SePayApiKey.Trim()
                                     : "Apikey tiemhoamuaxuan"; // Mã dự phòng

                if (string.IsNullOrEmpty(apiKey) || apiKey.Trim() != expectedKey.Trim())
                {
                    return new HttpStatusCodeResult(401, "Unauthorized - Sai API Key");
                }

                // 2. ĐỌC DỮ LIỆU JSON TỪ SEPAY
                string jsonContent;
                using (var reader = new StreamReader(Request.InputStream))
                {
                    jsonContent = reader.ReadToEnd();
                }

                // Chuyển chuỗi JSON thành đối tượng để dễ bóc tách
                var payload = JObject.Parse(jsonContent);

                // 3. LẤY THÔNG TIN GIAO DỊCH
                string content = payload["content"]?.ToString(); // Nội dung CK (VD: "Thanh toan don hang DH1007")
                string transferType = payload["transferType"]?.ToString(); // "in" là tiền vào, "out" là tiền ra
                decimal transferAmount = 0m;
                decimal.TryParse(
                    payload["transferAmount"]?.ToString(),
                    NumberStyles.Any,
                    CultureInfo.InvariantCulture,
                    out transferAmount);

                // 4. XỬ LÝ KHI CÓ TIỀN VÀO
                if (transferType == "in" && transferAmount > 0 && !string.IsNullOrEmpty(content))
                {
                    // Tìm mã đơn hàng trong nội dung chuyển khoản (VD: DH1007, DH123456)
                    // Regex này sẽ tìm chuỗi bắt đầu bằng DH và theo sau là các số
                    var match = Regex.Match(content, @"DH\d+");

                    if (match.Success)
                    {
                        string orderCode = match.Value;

                        // Tìm đơn hàng trong Database
                        var order = db.Orders.FirstOrDefault(o => o.OrderCode == orderCode);

                        // Nếu tìm thấy đơn, đơn đang ở trạng thái "Chờ thanh toán" và số tiền chuyển >= tổng tiền đơn hàng
                        if (order != null && order.Status == "Chờ thanh toán" && transferAmount >= order.TotalAmount)
                        {
                            // Đổi trạng thái thành "Đã thanh toán"
                            order.Status = "Đã thanh toán";
                            db.SaveChanges();
                        }
                    }
                }

                // 5. PHẢN HỒI 200 OK ĐỂ SEPAY BIẾT ĐÃ NHẬN ĐƯỢC TIN
                return new HttpStatusCodeResult(200);
            }
            catch (Exception ex)
            {
                // Nếu có lỗi (sai JSON, lỗi DB...), trả về 500 để SePay gửi lại sau
                return new HttpStatusCodeResult(500, ex.Message);
            }
        }
    }
}