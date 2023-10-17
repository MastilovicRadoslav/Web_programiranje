using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebShopApplication.Models;
using static WebShopApplication.Models.User;

namespace WebShopApplication.Controllers
{
    public class AdminController : Controller
    {
        public ActionResult Register()
        {
            return View();
        }

        public ActionResult ReviewProducts()
        {
            return View();
        }
        public ActionResult SellersCustomers()
        {
            return View();
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
                Birth = date.Date.ToString("dd-MM-yyyy")

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
            return View("SellersCustomers");
        }

        public ActionResult ReviewProfile()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ModifyUser(string username)
        {
            TempData["username"] = username;
            return View();
        }

        public ActionResult TryModifyUser(string username, string password, string first_name, string last_name, string gender, string email, DateTime? date)
        {
            List<User> users = (List<User>)HttpContext.Application["users"];

            foreach (var u in users)
            {
                if (u.Username == username)
                {
                    if (!string.IsNullOrWhiteSpace(password))
                        u.Password = password;
                    if (!string.IsNullOrWhiteSpace(first_name))
                        u.First_Name = first_name;
                    if (!string.IsNullOrWhiteSpace(last_name))
                        u.Last_Name = last_name;
                    if (!string.IsNullOrWhiteSpace(email))
                        u.Email = email;

                    if (date.HasValue)
                    {
                        u.Birth = date.Value.Date.ToString();
                    }

                    if (gender == "male")
                    {
                        u.Gender = Genders.Male;
                    }
                    else
                    {
                        u.Gender = Genders.Female;
                    }
                }
            }

            HttpContext.Application["users"] = users;
            RAW.WriteUsers(users);
            return View("ReviewProducts");
        }

        public ActionResult Block(string username)
        {
            List<User> users = (List<User>)HttpContext.Application["users"];
            foreach (var u in users)
            {
                if (u.Username == username)
                {
                    u.Deleted = true;
                    if (u.Function == TypeOfFunction.Customer)
                    {
                        foreach (var IDorder in u.Order_list)
                        {
                            List<Order> orders = (List<Order>)HttpContext.Application["orders"];
                            foreach (var order in orders)
                            {
                                if (order.ID == IDorder && order.Order_Status == Order.Status.Active)
                                {
                                    var prod = ((List<Product>)HttpContext.Application["products"]).First(p => p.ID == order.Product);
                                    prod.Amount++;
                                    if (prod.Status == false)
                                    {
                                        prod.Status = true;
                                    }
                                }
                            }
                        }
                    }
                    break;
                }
            }


            HttpContext.Application["users"] = users;
            RAW.WriteUsers(users);
            RAW.WriteProducts((List<Product>)HttpContext.Application["products"]);
            return View("SellersCustomers");
        }

        public ActionResult SearchMethodUsers(string first_name, string last_name, DateTime? minimum, DateTime? maximum, string function)
        {
            List<User> foreachList = new List<User>(((List<User>)HttpContext.Application["users"]).Where(usr => usr.Function == TypeOfFunction.Seller || usr.Function == TypeOfFunction.Customer));

            List<User> removingList = new List<User>(((List<User>)HttpContext.Application["users"]).Where(usr => usr.Function == TypeOfFunction.Seller || usr.Function == TypeOfFunction.Customer));


            if (first_name.Trim() != String.Empty)
            {
                foreach (var f in foreachList)
                {
                    if (f.First_Name != first_name.Trim())
                        removingList.Remove(f);

                    if (removingList.Count == 0)
                        break;
                }

            }

            if (last_name.Trim() != String.Empty)
            {
                foreach (var f in foreachList)
                {
                    if (f.Last_Name != last_name.Trim())
                        removingList.Remove(f);

                    if (removingList.Count == 0)
                        break;
                }

            }

            if (minimum.HasValue)
            {
                foreach (var f in foreachList)
                {
                    if (DateTime.Parse(f.Birth) < minimum)
                        removingList.Remove(f);

                    if (removingList.Count == 0)
                        break;
                }
            }

            if (maximum.HasValue)
            {
                foreach (var f in foreachList)
                {
                    if (DateTime.Parse(f.Birth) > maximum)
                        removingList.Remove(f);

                    if (removingList.Count == 0)
                        break;
                }
            }

            if (function.Trim() != String.Empty)
            {
                TypeOfFunction func;

                if (function == "customer")
                {
                    func = TypeOfFunction.Customer;
                }
                else if (function == "seller")
                {
                    func = TypeOfFunction.Seller;
                }
                else
                {//sve brise iz liste ako je bilo sta drugo
                    func = TypeOfFunction.Admin;
                }

                foreach (var f in foreachList)
                {
                    if (f.Function != func)
                        removingList.Remove(f);

                    if (removingList.Count == 0)
                        break;
                }
            }


            TempData["Show"] = removingList;

            return View("SellersCustomers");
        }



        public ActionResult SortMethodUsers(string sortype, string sortproperty)
        {
            List<User> sortList = new List<User>((List<User>)HttpContext.Application["users"]).Where(u => u.Function == TypeOfFunction.Customer || u.Function == TypeOfFunction.Seller).ToList();

            if (sortype.Trim() == "ASC")
            {
                if (sortproperty == "name")
                    sortList = sortList.OrderBy(s => s.First_Name).ToList();
                else if (sortproperty == "birthday")
                    sortList = sortList.OrderBy(s => DateTime.Parse(s.Birth)).ToList();
                else
                    sortList = sortList.OrderBy(s => s.Function).ToList();
            }
            else
            {
                if (sortproperty == "name")
                    sortList = sortList.OrderByDescending(s => s.First_Name).ToList();
                else if (sortproperty == "birthday")
                    sortList = sortList.OrderByDescending(s => DateTime.Parse(s.Birth)).ToList();
                else
                    sortList = sortList.OrderByDescending(s => s.Function).ToList();
            }

            TempData["Show"] = sortList;
            return View("SellersCustomers");
        }

        public ActionResult SearchMethodProduct(string searchtype)
        {
            List<Product> foreachList = new List<Product>((List<Product>)HttpContext.Application["products"]);

            List<Product> removingList = new List<Product>((List<Product>)HttpContext.Application["products"]);

            if (searchtype == "AVL")
            {
                foreach (var f in foreachList)
                {
                    if (f.Status != true)
                        removingList.Remove(f);

                    if (removingList.Count == 0)
                        break;
                }
            }
            else
            {
                foreach (var f in foreachList)
                {
                    if (f.Status == true)
                        removingList.Remove(f);

                    if (removingList.Count == 0)
                        break;
                }
            }

            TempData["Show"] = removingList;

            return View("ReviewProducts");

        }

        public ActionResult SortMethodProducts(string sortype, string sortproperty)
        {
            List<Product> sortList = new List<Product>((List<Product>)HttpContext.Application["products"]);

            if (sortype.Trim() == "ASC")
            {
                if (sortproperty == "name")
                    sortList = sortList.OrderBy(s => s.Name).ToList();
                else if (sortproperty == "price")
                    sortList = sortList.OrderBy(s => s.Price).ToList();
                else
                    sortList = sortList.OrderBy(s => DateTime.Parse(s.Date_product)).ToList();
            }
            else
            {
                if (sortproperty == "name")
                    sortList = sortList.OrderByDescending(s => s.Name).ToList();
                else if (sortproperty == "price")
                    sortList = sortList.OrderByDescending(s => s.Price).ToList();
                else
                    sortList = sortList.OrderByDescending(s => DateTime.Parse(s.Date_product)).ToList();
            }

            TempData["Show"] = sortList;
            return View("ReviewProducts");
        }

        public ActionResult ReviewOrders()
        {
            return View();
        }
        public ActionResult ModifyProduct(string IDOfProduct)
        {
            ViewBag.IDOfProduct = IDOfProduct;
            return View();
        }


        public ActionResult TryModifyProduct(string IDOfProduct, string name, string description, string city, HttpPostedFileBase image = null, UInt64 price = 0, UInt64 amount = 0)
        {
            List<Product> products = (List<Product>)HttpContext.Application["products"];

            foreach (var p in products)
            {
                if (p.ID == IDOfProduct)
                {
                    if (!string.IsNullOrWhiteSpace(name))
                        p.Name = name;
                    if (price != 0)
                        p.Price = price;
                    if (amount != 0)
                    {
                        p.Amount = amount;
                        p.Status = true;
                    }

                    if (!string.IsNullOrWhiteSpace(description))
                        p.Description = description;


                    if (!string.IsNullOrWhiteSpace(city))
                        p.City = city;

                    if (image != null)
                    {
                        p.Image = GetBytes(image);
                    }
                    TempData["Msg"] = "";
                    break;
                }
            }

            HttpContext.Application["products"] = products;
            RAW.WriteProducts(products);
            return View("ReviewProducts");
        }

        public byte[] GetBytes(HttpPostedFileBase image)
        {
            MemoryStream target = new MemoryStream();
            image.InputStream.CopyTo(target);
            byte[] data = target.ToArray();
            return data;
        }

        public ActionResult DeleteProduct(string IDOfProduct)
        {
            List<Product> productList = (List<Product>)HttpContext.Application["products"];

            foreach (var p in productList)
            {
                if (p.ID == IDOfProduct)
                {
                    p.Deleted = true;
                    HttpContext.Application["products"] = productList;
                    RAW.WriteProducts(productList);

                    return View("ReviewProducts");
                }
            }

            return View("ReviewProducts");
        }

        public ActionResult OrderFinished(string IDOfOrder)
        {
            List<Order> orderList = (List<Order>)HttpContext.Application["orders"];

            foreach (var o in orderList)
            {
                if (o.ID == IDOfOrder)
                {
                    o.Order_Status = Order.Status.Finished;
                    HttpContext.Application["orders"] = orderList;
                    RAW.WriteOrders(orderList);

                    break;
                }
            }

            return View("ReviewOrders");
        }
        public ActionResult OrderDeclined(string IDOfOrder)
        {
            List<Order> orderList = (List<Order>)HttpContext.Application["orders"];

            foreach (var o in orderList)
            {
                if (o.ID == IDOfOrder)
                {
                    if (o.Order_Status == Order.Status.Active)
                    {
                        var prod = ((List<Product>)HttpContext.Application["products"]).First(p => p.ID == o.Product);
                        prod.Amount++;
                        if (prod.Status == false)
                        {
                            prod.Status = true;
                        }
                        RAW.WriteProducts((List<Product>)HttpContext.Application["products"]);
                    }
                    o.Order_Status = Order.Status.Declined;
                    HttpContext.Application["orders"] = orderList;
                    RAW.WriteOrders(orderList);

                    break;
                }
            }

            return View("ReviewOrders");
        }

        public ActionResult ApplyVisibility(string IDOfReview)
        {
            List<Review> rList = (List<Review>)HttpContext.Application["reviews"];

            foreach (var r in rList)
            {
                if (r.ID == IDOfReview)
                {
                    r.Deleted = false;
                }
            }

            HttpContext.Application["reviews"] = rList;
            RAW.WriteReview(rList);

            return View("Reviews");

        }

        public ActionResult DenyVisibility(string IDOfReview)
        {
            List<Review> rList = (List<Review>)HttpContext.Application["reviews"];

            foreach (var r in rList)
            {
                if (r.ID == IDOfReview)
                {
                    r.Deleted = true;
                }
            }

            HttpContext.Application["reviews"] = rList;
            RAW.WriteReview(rList);

            return View("Reviews");

        }

        public ActionResult Reviews()
        {
            return View();
        }
    }
}