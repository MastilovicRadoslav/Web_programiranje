using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using WebShopApplication.Models;

namespace WebShopApplication
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            HttpContext.Current.Application["users"] = RAW.ReadUsers();  //metode koje omogucavaju da citam iz fajlova xtml
            HttpContext.Current.Application["products"] = RAW.ReadProducts();
            HttpContext.Current.Application["orders"] = RAW.ReadOrders();
            HttpContext.Current.Application["reviews"] = RAW.ReadReview();


            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }
    }
}
