using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace WebShopApplication.Models
{
    public static class RAW
    {

        public static void WriteUsers(List<User> users)
        {
            var pth = HttpContext.Current.Server.MapPath("/App_Data/Users.xml");
            XmlSerializer serializer = new XmlSerializer(typeof(List<User>));
            FileStream fs = new FileStream(pth, FileMode.Create);

            serializer.Serialize(fs, users);
            fs.Close();
        }

        public static List<User> ReadUsers()
        {
            var pth = HttpContext.Current.Server.MapPath("/App_Data/Users.xml");
            XmlSerializer serializer = new XmlSerializer(typeof(List<User>));

            if (FileExist(pth) == false)
            {
                WriteUsers(new List<User>());
            }

            using (FileStream stream = File.OpenRead(pth))
            {
                List<User> deserializedList = (List<User>)serializer.Deserialize(stream);
                return deserializedList;
            }

        }

        public static void WriteProducts(List<Product> products)
        {
            var pth = HttpContext.Current.Server.MapPath("/App_Data/Products.xml");
            XmlSerializer serializer = new XmlSerializer(typeof(List<Product>));
            FileStream fs = new FileStream(pth, FileMode.Create);

            serializer.Serialize(fs, products);
            fs.Close();
        }

        public static List<Product> ReadProducts()
        {
            var pth = HttpContext.Current.Server.MapPath("/App_Data/Products.xml");
            XmlSerializer serializer = new XmlSerializer(typeof(List<Product>));

            if (FileExist(pth) == false)
            {
                WriteProducts(new List<Product>());
            }

            using (FileStream stream = File.OpenRead(pth))
            {
                List<Product> deserializedList = (List<Product>)serializer.Deserialize(stream);
                return deserializedList;
            }

        }

        public static void WriteOrders(List<Order> workouts)
        {
            var pth = HttpContext.Current.Server.MapPath("/App_Data/Orders.xml");
            XmlSerializer serializer = new XmlSerializer(typeof(List<Order>));
            FileStream fs = new FileStream(pth, FileMode.Create);

            serializer.Serialize(fs, workouts);
            fs.Close();
        }

        public static List<Order> ReadOrders()
        {
            var pth = HttpContext.Current.Server.MapPath("/App_Data/Orders.xml");
            XmlSerializer serializer = new XmlSerializer(typeof(List<Order>));

            if (FileExist(pth) == false)
            {
                WriteOrders(new List<Order>());
            }

            using (FileStream stream = File.OpenRead(pth))
            {
                List<Order> deserializedList = (List<Order>)serializer.Deserialize(stream);
                return deserializedList;
            }

        }

        public static void WriteReview(List<Review> comments)
        {
            var pth = HttpContext.Current.Server.MapPath("/App_Data/Reviews.xml");
            XmlSerializer serializer = new XmlSerializer(typeof(List<Review>));
            FileStream fs = new FileStream(pth, FileMode.Create);

            serializer.Serialize(fs, comments);
            fs.Close();
        }

        public static List<Review> ReadReview()
        {
            var pth = HttpContext.Current.Server.MapPath("/App_Data/Reviews.xml");
            XmlSerializer serializer = new XmlSerializer(typeof(List<Review>));

            if (FileExist(pth) == false)
            {
                WriteReview(new List<Review>());
            }

            using (FileStream stream = File.OpenRead(pth))
            {
                List<Review> deserializedList = (List<Review>)serializer.Deserialize(stream);
                return deserializedList;
            }

        }

        public static bool FileExist(string pth)
        {
            if (File.Exists(pth))
                return true;

            return false;
        }
    }

}