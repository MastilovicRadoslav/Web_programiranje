using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebShopApplication.Models
{
    public class Review
    {
        public string ID;
        public string Reviewer;
        public string Product;
        public string Title;
        public string Content;
        public byte[] Image;
        public bool Deleted = true;

    }
}