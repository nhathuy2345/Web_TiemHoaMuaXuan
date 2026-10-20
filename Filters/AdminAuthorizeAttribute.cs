using System.Web;
using System.Web.Mvc;
using Web_TiemHoaMuaXuan.Models.Entities;

namespace Web_TiemHoaMuaXuan.Filters
{
    /// <summary>
    /// Custom Authorize Attribute — Chỉ cho phép Admin truy cập
    /// Nếu chưa login → chuyển về /Account/Login
    /// Nếu login nhưng không phải admin → hiện lỗi 403
    /// </summary>
    public class AdminAuthorizeAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            // Lấy user từ Session
            var user = HttpContext.Current.Session["User"] as AppUser;

            // ===== 1. Chưa đăng nhập =====
            if (user == null)
            {
                // Lưu URL hiện tại để sau khi login quay lại
                string returnUrl = filterContext.HttpContext.Request.RawUrl;

                filterContext.Result = new RedirectResult(
                    "~/Account/Login?returnUrl=" + HttpUtility.UrlEncode(returnUrl)
                );
                return;
            }

            // ===== 2. Đã login nhưng KHÔNG phải Admin =====
            if (!user.IsAdmin)
            {
                // Trả về lỗi 403 Forbidden
                filterContext.Result = new HttpStatusCodeResult(403, "Bạn không có quyền truy cập trang quản trị");
                return;
            }

            // ===== 3. Đã login + Là Admin → Cho phép =====
            base.OnActionExecuting(filterContext);
        }
    }
}