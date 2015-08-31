using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;


namespace SUAMVC
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }

        public void Session_Start()
        {
            Session.Timeout = 600;
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            var p = Request.Path.ToLower().Trim();
            if (p.EndsWith("/crystalimagehandler.aspx") && p != "/crystalimagehandler.aspx")
            {
                var fullPath = Request.Url.AbsoluteUri.ToLower();
                var index = fullPath.IndexOf("/crystalimagehandler.aspx");
                Response.Redirect(fullPath.Substring(index));
            }
        }
    }
}
