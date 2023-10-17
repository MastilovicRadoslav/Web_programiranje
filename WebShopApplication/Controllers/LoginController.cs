using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebShopApplication.Models;
using static WebShopApplication.Models.User;

namespace WebShopApplication.Controllers
{
    public class LoginController : Controller
    {
        public ActionResult Login()
        {
            return View();
        }

        public ActionResult Register()
        {
            return View();
        }
        public ActionResult TryLogInUser(string username, string password)
        {
            List<User> users = (List<User>)HttpContext.Application["users"];

            foreach (var us in users)
            {
                if (us.Deleted == false && us.Username == username && us.Password == password)
                {
                    Session["Login"] = us;

                    if (us.Function == TypeOfFunction.Admin)
                    {
                        return RedirectToAction("ReviewProducts", "Admin");
                    }
                    else if (us.Function == TypeOfFunction.Seller)
                    {
                        return RedirectToAction("ReviewProducts", "Unlogged");
                    }
                    else
                    {
                        return RedirectToAction("ReviewProducts", "Unlogged");
                    }

                }
            }
            TempData["Msg"] = "Incorrect Username or Password";
            return View("Login");

        }

        public ActionResult TryRegisterUser(string username, string password, string first_name, string last_name, string gender, string email, DateTime date, string function)
        {
            List<User> users = (List<User>)HttpContext.Application["users"];


            foreach (var us in users)
            {
                if (us.Username == username)
                {
                    TempData["Msg"] = "Username already exists";
                    return View("Register");
                }
            }

            User u = new User
            {
                Username = username,
                Password = password,
                First_Name = first_name,
                Last_Name = last_name,
                Email = email,
                Birth = date.Date.ToString()

            };

            if (function == "admin")
            {
                u.Function = TypeOfFunction.Admin;
            }
            else if (function == "customer")
            {
                u.Function = TypeOfFunction.Customer;
                u.Favorite_products = new List<string>();
                u.Order_list = new List<string>();
            }
            else
            {
                u.Function = TypeOfFunction.Seller;
                u.Published_products = new List<string>();
            }

            if (gender == "male")
            {
                u.Gender = Genders.Male;
            }
            else
            {
                u.Gender = Genders.Female;
            }

            users.Add(u);

            RAW.WriteUsers(users);
            return View("Login");
        }



        public ActionResult LogOut()
        {
            Session["Login"] = null;
            return View("Login");
        }
    }
}