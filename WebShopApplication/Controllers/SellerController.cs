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
    public class SellerController : Controller
    {
        // GET: Seller
        public ActionResult ReviewProducts()
        {
            return View();
        }

        public ActionResult AddProduct()
        {
            return View();
        }

        [HttpPost]
        public ActionResult TryAddProduct(string name, UInt64 price, UInt64 amount, string description, HttpPostedFileBase image, string city)
        {
            List<Product> products = (List<Product>)HttpContext.Application["products"];

            Product p = new Product
            {
                ID = GetId(),
                Name = name,
                Price = price,
                Amount = amount,
                Description = description,
                Date_product = DateTime.Now.Date.ToString(),
                Image = GetBytes(image),
                City = city,
                Reviews = new List<string>(),
                Status = true,
                Owner = ((User)Session["Login"]).Username
            };

            if (amount <= 0)
                p.Status = false;
            else
                p.Status = true;

            List<User> userList = (List<User>)HttpContext.Application["users"];
            foreach (var u in userList)
            {
                if (u.Username == ((User)Session["Login"]).Username)
                {
                    u.Published_products.Add(p.ID);
                    break;
                }
            }
            RAW.WriteUsers(userList);

            products.Add(p);
            RAW.WriteProducts(products);
            return View("ReviewProducts");
        }

        public ActionResult ModifyProduct(string IDOfProduct)
        {
            TempData["ID"] = IDOfProduct;
            return View();
        }

        public ActionResult TryModifyProduct(string IDOfProduct, string name, string description, string city, HttpPostedFileBase image = null, UInt64 price = 0, UInt64 amount = 0)
        {
            List<Product> products = (List<Product>)HttpContext.Application["products"];

            foreach (var p in products)
            {
                if (p.ID == IDOfProduct && p.Ordered == false)
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
                TempData["Msg"] = "Could't modify product, someone ordered that Product!";
            }

            HttpContext.Application["products"] = products;
            RAW.WriteProducts(products);
            return View("ReviewProducts");
        }


        public static string ConvertBytesToImage(byte[] arr)
        {
            string imreBase64Data = Convert.ToBase64String(arr);
            string imgDataURL = string.Format("data:image/png;base64,{0}", imreBase64Data);
            return imgDataURL;
        }

        public byte[] GetBytes(HttpPostedFileBase image)
        {
            MemoryStream target = new MemoryStream();
            image.InputStream.CopyTo(target);
            byte[] data = target.ToArray();
            return data;
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

        public ActionResult Delete(string IDOfProduct)
        {
            List<Product> productList = (List<Product>)HttpContext.Application["products"];

            foreach (var p in productList)
            {
                if (p.ID == IDOfProduct && p.Ordered == false)
                {
                    p.Deleted = true;
                    HttpContext.Application["products"] = productList;
                    RAW.WriteProducts(productList);

                    return View("ReviewProducts");
                }
            }

            TempData["Msg"] = "Somene Ordered That Product";
            return View("ReviewProducts");
        }

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

        public ActionResult SearchMethod(string searchtype)
        {

            List<Product> foreachList = new List<Product>();

            List<Product> removingList = new List<Product>();

            foreach (Product p in (List<Product>)HttpContext.Application["products"])
            {
                if (((User)Session["Login"]).Published_products.Contains(p.ID) && p.Deleted == false)
                {
                    foreachList.Add(p);
                    removingList.Add(p);
                }

            }

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

        public ActionResult SortMethod(string sortype, string sortproperty)
        {

            List<Product> sortList = new List<Product>();

            foreach (Product p in (List<Product>)HttpContext.Application["products"])
            {
                if (((User)Session["Login"]).Published_products.Contains(p.ID) && p.Deleted == false)
                {
                    sortList.Add(p);
                }

            }

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
    }
}