using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebShopApplication.Models;

namespace WebShopApplication.Controllers
{
    public class UnloggedController : Controller
    {
        // GET: Unlogged
        public ActionResult ReviewProducts()
        {
            if ((User)Session["Login"] != null && ((User)Session["Login"]).Function == Models.User.TypeOfFunction.Customer)
            {
                TempData["customer"] = "yes";
            }

            return View();
        }

        public ActionResult SearchMethod(string name, string city, UInt64 minimum = 0, UInt64 maximum = 0)
        {
            List<Product> foreachList = new List<Product>((List<Product>)HttpContext.Application["products"]);

            List<Product> removingList = new List<Product>((List<Product>)HttpContext.Application["products"]);


            if (name.Trim() != String.Empty)
            {
                foreach (var f in foreachList)
                {
                    if (f.Name != name.Trim())
                        removingList.Remove(f);

                    if (removingList.Count == 0)
                        break;
                }

            }

            if (city.Trim() != String.Empty)
            {
                foreach (var f in foreachList)
                {
                    if (f.City != city.Trim())
                        removingList.Remove(f);

                    if (removingList.Count == 0)
                        break;
                }
            }

            if (minimum != 0)
            {
                foreach (var f in foreachList)
                {
                    if (f.Price < minimum)
                        removingList.Remove(f);

                    if (removingList.Count == 0)
                        break;
                }
            }

            if (maximum != 0)
            {
                foreach (var f in foreachList)
                {
                    if (f.Price > maximum)
                        removingList.Remove(f);

                    if (removingList.Count == 0)
                        break;
                }
            }

            TempData["Show"] = removingList;

            return View("ReviewProducts");

        }

        public ActionResult SortMethod(string sortype, string sortproperty)
        {
            List<Product> sortList = new List<Product>((List<Product>)HttpContext.Application["products"]);

            if (sortype.Trim() == "ASC")
            {
                if (sortproperty == "name")
                    sortList = sortList.OrderBy(s => s.Name).ToList();
                else if (sortproperty == "price")
                    sortList = sortList.OrderBy(s => s.Price).ToList();
                else
                    sortList = sortList.OrderBy(s => s.Date_product).ToList();
            }
            else
            {
                if (sortproperty == "name")
                    sortList = sortList.OrderByDescending(s => s.Name).ToList();
                else if (sortproperty == "price")
                    sortList = sortList.OrderByDescending(s => s.Price).ToList();
                else
                    sortList = sortList.OrderByDescending(s => s.Date_product).ToList();
            }

            TempData["Show"] = sortList;
            return View("ReviewProducts");
        }
    }
}