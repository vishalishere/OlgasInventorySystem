using System;

namespace InventorySystem.Models
{
    public class Product
    {
        public const string ExpiredMessage = "Product expired";
        public const string FreshMessage = "Product is fresh";

        public string Label { get; set; }
        public string Type { get; set; }
        public string Expiration { get; set; }
        public string Notification { get; set; }

        public Product(string label, string type, string expiration)
        {
            Label = label;
            Type = type;
            Expiration = expiration;
            Notification = DateTime.Parse(Expiration) < DateTime.Now ? ExpiredMessage : FreshMessage;
        }

        public bool Equals(Product anotherProduct)
        {
            if (Label == anotherProduct.Label && Type == anotherProduct.Type && Expiration == anotherProduct.Expiration)
                return true;
            return false;
        }
    }
}