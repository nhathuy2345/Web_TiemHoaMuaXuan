using System.Web;
using System.Web.Mvc;
using Web_TiemHoaMuaXuan.Models.Entities;

namespace Web_TiemHoaMuaXuan.Filters
{
    /// <summary>
    /// Lớp này dùng để chặn người dùng truy cập trái phép vào trang Admin
    /// </summary>
    public class AdminAuthorizeAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            // Lấy thông tin user từ Session
            var user = HttpContext.Current.Session["User"] as AppUser;

            // 1. Nếu chưa đăng nhập -> Đá về trang Đăng nhập
            if (user == null)
            {
                string returnUrl = filterContext.HttpContext.Request.RawUrl;
                filterContext.Result = new RedirectResult("~/Account/Login?returnUrl=" + HttpUtility.UrlEncode(returnUrl));
                return;
            }

            // 2. Nếu đã đăng nhập nhưng không phải Admin -> Báo lỗi 403 (Forbidden)
            if (!user.IsAdmin)
            {
                filterContext.Result = new HttpStatusCodeResult(403, "Bạn không có quyền truy cập trang quản trị.");
                return;
            }

            base.OnActionExecuting(filterContext);
        }
    }
}