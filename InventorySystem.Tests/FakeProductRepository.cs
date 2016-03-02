using System;
using System.Collections.Generic;
using System.Linq;
using InventorySystem.Models;
using InventorySystem.Services;

namespace InventorySystem.Tests
{
    public class FakeProductRepository : IProductRepository
    {
        public List<Product> Products = new List<Product>{new Product("Milk", "Drink", new DateTime(2017, 11, 5)),
                        new Product("Orange Juice", "Drink", new DateTime(2014, 2, 3))};

        public IEnumerable<Product> GetAllProducts()
        {
            return Products;
        }

        public bool SaveProduct(IEnumerable<Product> productsToSave)
        {
            foreach (var product in productsToSave)
            {
                var item = new Product(product.Label, product.Type, DateTime.Parse(product.Expiration));
                Products.Add(item);
            }
            return true;
        }

        public IEnumerable<Product> GetProductByLabel(string label)
        {
            return Products.Where(p => string.Equals(p.Label, label, StringComparison.CurrentCultureIgnoreCase));
        }

        public bool RemoveProduct(string label)
        {
            Products.RemoveAll(p => string.Equals(p.Label, label, StringComparison.CurrentCultureIgnoreCase));
            return true;
        }

        public bool UpdateProduct(string label, Product updateTo)
        {
            foreach (var p in Products.Where(p => string.Equals(p.Label, label, StringComparison.CurrentCultureIgnoreCase)))
            {
                p.Label = string.IsNullOrEmpty(updateTo.Label) ? p.Label : updateTo.Label;
                p.Expiration = string.IsNullOrEmpty(updateTo.Expiration) ? p.Expiration : updateTo.Expiration;
                p.Type = string.IsNullOrEmpty(updateTo.Type) ? p.Type : updateTo.Type;
            }
            return true;
        }

        public List<string> GetAllMsgsSent()
        {
            return new List<string>() {"Item with label Orange Juice expired."};
        }
    }
}
