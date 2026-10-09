using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Web_TiemHoaMuaXuan.Startup))]

namespace Web_TiemHoaMuaXuan
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // Không cần auth — chỉ để VS hết báo lỗi
        }
    }
}