using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebShopApplication.Models
{
    public class User
    {
        public string Username;
        public string Password;
        public string First_Name;
        public string Last_Name;
        public string Email;
        public Genders Gender;
        public string Birth;
        public TypeOfFunction Function;
        public List<string> Order_list;
        public List<string> Favorite_products;
        public List<string> Published_products;
        public bool Deleted = false;
        public enum Genders
        {
            Male,
            Female
        }

        public enum TypeOfFunction
        {
            Admin,
            Customer,
            Seller
        }
    }
}