using System;
using InventorySystem.Controllers;
using Microsoft.AspNet.SignalR;

namespace InventorySystem.Models
{
    public class Product
    {
        public string Label { get; set; }
        public string Type { get; set; }
        public string Expiration { get; set; }
        public string Notification { get; set; }

        public Product(string label, string type, DateTime expiration)
        {
            Label = label;
            Type = type;
            Expiration = expiration.ToShortDateString();
            Notification = expiration < DateTime.Now ? "Product expired" : "Product is fresh";
        }

        public bool Equals(Product anotherProduct)
        {
            if (Label == anotherProduct.Label && Type == anotherProduct.Type && Expiration == anotherProduct.Expiration)
                return true;
            return false;
        }
    }
}