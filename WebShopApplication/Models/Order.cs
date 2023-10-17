using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebShopApplication.Models
{
    public class Order
    {
        public string ID;
        public string Product;
        public string Customer;
        public string Date_of_order;
        public Status Order_Status;
        public bool Deleted = false;

        public enum Status
        {
            Active,
            Finished,
            Declined
        }
    }
}