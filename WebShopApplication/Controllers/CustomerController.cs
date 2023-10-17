using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;
using WebShopApplication.Models;
using static WebShopApplication.Models.User;

namespace WebShopApplication.Controllers
{
    public class CustomerController : Controller
    {
        public ActionResult ModifyUser()
        {
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
            return View("ReviewProfile");
        }

        public ActionResult ReviewProfile()
        {
            return View();
        }

        public ActionResult AddToFavouriteList(string IDOfProduct)
        {
            List<User> users = (List<User>)HttpContext.Application["users"];

            foreach (var u in users)
            {
                if (u.Username == ((User)Session["Login"]).Username)
                {
                    u.Favorite_products.Add(IDOfProduct);
                    break;
                }
            }

            RAW.WriteUsers(users);

            return View("ReviewFavouriteList");


        }

        public ActionResult ReviewFavouriteList()
        {
            return View();
        }

        public ActionResult OrderProduct(string IDOfProduct)
        {
            List<Product> productList = (List<Product>)HttpContext.Application["products"];

            List<User> userList = (List<User>)HttpContext.Application["users"];

            List<Order> orders = (List<Order>)HttpContext.Application["orders"];


            foreach (var p in productList)
            {
                if (p.ID == IDOfProduct)
                {
                    p.Amount--;
                    p.Ordered = true;
                    if (p.Amount == 0)
                    {
                        p.Status = false;
                    }
                    break;

                }
            }



            Order o = new Order
            {
                ID = GetId(),
                Product = IDOfProduct,
                Customer = ((User)Session["Login"]).Username,
                Date_of_order = DateTime.Now.Date.ToString(),
                Order_Status = Order.Status.Active,

            };

            foreach (var us in userList)
            {
                if (us.Username == ((User)Session["Login"]).Username)
                {
                    us.Order_list.Add(o.ID);
                    break;
                }
            }

            orders.Add(o);
            RAW.WriteOrders(orders);

            RAW.WriteProducts(productList);
            RAW.WriteUsers(userList);

            return RedirectToAction("ReviewOrders");

        }

        public string GetId()
        {
            using (var rng = new RNGCryptoServiceProvider())
            {
                var bit_count = (9 * 6);
                var byte_count = ((bit_count + 7) / 8);
                var bytes = new byte[byte_count];
                rng.GetBytes(bytes);
                return Convert.ToBase64String(bytes);
            }
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

        public ActionResult CreateAReview(string IDOfOrder)
        {
            ViewBag.IDOfOrder = IDOfOrder;
            return View();
        }

        public ActionResult ReviewOrders()
        {
            return View();
        }

        [HttpPost]
        public ActionResult TryReview(string customer, string IDOfOrder, string title, string content, HttpPostedFileBase image)
        {

            List<Order> orderList = (List<Order>)HttpContext.Application["orders"];

            List<Review> reviews = (List<Review>)HttpContext.Application["reviews"];

            List<Product> products = (List<Product>)HttpContext.Application["products"];

            string productID = "";
            foreach (var o in orderList)
            {
                if (o.ID == IDOfOrder)
                {
                    productID = o.Product;
                    break;
                }
            }

            Review r = new Review
            {
                ID = GetId(),
                Reviewer = customer,
                Product = productID,
                Title = title,
                Content = content,
            };

            if (image != null)
            {
                r.Image = GetBytes(image);
            }

            foreach (var p in products)
            {
                if (p.ID == r.Product)
                {
                    p.Reviews = new List<string>();
                    p.Reviews.Add(r.ID);
                    RAW.WriteProducts(products);
                    break;
                }
            }

            reviews.Add(r);
            RAW.WriteReview(reviews);
            return View("Reviews");
        }

        public ActionResult Reviews()
        {
            return View();
        }

        public byte[] GetBytes(HttpPostedFileBase image)
        {
            MemoryStream target = new MemoryStream();
            image.InputStream.CopyTo(target);
            byte[] data = target.ToArray();
            return data;
        }

        public ActionResult ModifyReview(string IDOfReview)
        {
            TempData["ID"] = IDOfReview;
            return View();
        }

        public ActionResult TryModifyReview(string IDOfReview, string title, string content, HttpPostedFileBase image)
        {
            List<Review> reviews = (List<Review>)HttpContext.Application["reviews"];

            foreach (var r in reviews)
            {
                if (r.ID == IDOfReview)
                {
                    if (!string.IsNullOrWhiteSpace(title))
                        r.Title = title;


                    if (!string.IsNullOrWhiteSpace(content))
                        r.Content = content;


                    if (image != null)
                    {
                        r.Image = GetBytes(image);
                    }

                    break;

                }
            }

            HttpContext.Application["reviews"] = reviews;
            RAW.WriteReview(reviews);
            return View("Reviews");
        }

        public ActionResult Delete(string IDOfReview)
        {
            List<Review> reviewList = (List<Review>)HttpContext.Application["reviews"];

            foreach (var r in reviewList)
            {
                if (r.ID == IDOfReview)
                {
                    r.Deleted = true;
                    HttpContext.Application["reviews"] = reviewList;
                    RAW.WriteReview(reviewList);

                    break;
                }
            }
            return View("Reviews");
        }
    }
}
