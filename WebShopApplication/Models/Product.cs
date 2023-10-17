using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebShopApplication.Models
{
    public class Product
    {
        public string ID;
        public string Name;
        public UInt64 Price;
        public UInt64 Amount;
        public string Description;
        public byte[] Image;
        public string Date_product;
        public string City;
        public List<string> Reviews;
        public bool Status;
        public string Owner;
        public bool Ordered = false;
        public bool Deleted = false;
    }
}