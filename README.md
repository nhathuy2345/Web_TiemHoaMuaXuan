DỰ ÁN WEBSITE BÁN HOA SPRINGFLOWER

NHÓM EM CHÀO CÔ Ạ !,

BƯỚC 1: CÀI ĐẶT DATABASE
1. Mở SQL Server Management Studio (SSMS) trên máy Cô.
2. Tạo một Database trống mới với tên là: TiemHoaDB
3. Mở file Database/databasetiemhoadb.sql (nằm trong thư mục Database của dự án) bằng SSMS.
4. Bấm nút Execute (hoặc F5) để tạo bảng và nạp dữ liệu mẫu.

LƯU Ý QUAN TRỌNG (NẾU BỊ LỖI KẾT NỐI DATABASE):
- Dự án đang cấu hình kết nối mặc định với SQL Server LocalDB.
- Nếu máy Cô dùng SQL Server Express (tên server là .\SQLEXPRESS), Cô vui lòng mở file Web.config, tìm dòng connectionString và sửa Data Source=(localdb)\MSSQLLocalDB thành Data Source=.\SQLEXPRESS.
- Nếu máy Cô dùng Localhost, Cô sửa thành Data Source=localhost.

BƯỚC 2: MỞ VÀ CHẠY DỰ ÁN
1. Mở file Web_TiemHoaMuaXuan.sln bằng Visual Studio.
2. Nhấn phím F5 để chạy chương trình.

BƯỚC 3: TÀI KHOẢN ĐĂNG NHẬP MẪU
Tài khoản Quản trị (Admin) chạy dưới máy Localhost:
- Tài khoản: admin@gmail.com
- Mật khẩu: admin123

LINK WEB ONLINE
Nếu Cô muốn xem giao diện trực tiếp (không cần cài đặt), Cô có thể truy cập:
https://tiemhoamuaxuan.io.vn/

Tài khoản Quản trị (Admin) chạy trên Live Hosting:
- Tài khoản: admin@gmail.com
- Mật khẩu: 123456

Ghi chú: 
- Bản web trên hosting là phiên bản nhóm em đã triển khai lên server thật và test trực tiếp rất nhiều tính năng (thêm sản phẩm, đặt đơn hàng). Vì vậy, hiện tại dữ liệu trên hosting có thể nhiều hơn so với file database mẫu (databasetiemhoadb.sql) mà nhóm em nộp kèm dưới máy. Tuy nhiên, toàn bộ các chức năng của hệ thống là hoàn toàn giống nhau ạ.
- Trong code có phần thanh toán SePay Webhook. Hiện tại nhóm em đã tích hợp thành công trên Hosting. Khi chạy dưới máy Localhost, phần này sẽ hiển thị lỗi do không có URL công khai để cổng thanh toán gọi về. Mong Cô bỏ qua và thông cảm ạ.

Nhóm em có phát triển lên hosting thông qua tên miền .io.vn và đẩy SQL lên thông qua SmarterASP.NET.

Chúc Cô chấm bài vui vẻ ạ!
