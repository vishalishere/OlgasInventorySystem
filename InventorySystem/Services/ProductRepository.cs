using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using InventorySystem.Models;

namespace InventorySystem.Services
{
    public class ProductRepository
    {
        private static readonly List<string> MsgsSent = new List<string>();
        private static readonly ConcurrentDictionary<string, Product> Data = new ConcurrentDictionary<string, Product>(StringComparer.OrdinalIgnoreCase);

        public ProductRepository()
        {
            var expirationChecker = new Checker(CheckProductsForExpiration, 10);
        }

        public IEnumerable<Product> GetAllProducts()
        {
            return Data.Values;
        }

        public bool SaveProduct(IEnumerable<Product> productsToSave)
        {
            bool isSuccess = true;
            foreach (var product in productsToSave)
            {
                product.Notification = DateTime.Parse(product.Expiration) < DateTime.Now ? Product.ExpiredMessage : Product.FreshMessage;
                isSuccess = Data.TryAdd(product.Label, product);
            }
            return isSuccess;
        }

        public Product GetProductByLabel(string label)
        {
            if(Data.ContainsKey(label))
                return Data[label];
            return null;
        }

        public bool RemoveProduct(string label)
        {
            Product product;
            var isSuccess = Data.TryRemove(label, out product);
            
            if(isSuccess)
                Notification.SendMessage(string.Format("Product with label {0} was deleted.", label));

            return isSuccess;
        }

        public void RemoveAllProducts()
        {
            Data.Clear();
        }

        public bool UpdateProduct(string label, Product updateTo)
        {
            try
            {
                foreach (
                    var p in
                        Data.Where(
                            product => string.Equals(product.Key, label, StringComparison.CurrentCultureIgnoreCase)))
                {
                    p.Value.Expiration = string.IsNullOrEmpty(updateTo.Expiration)
                        ? p.Value.Expiration
                        : updateTo.Expiration;
                    p.Value.Type = string.IsNullOrEmpty(updateTo.Type) ? p.Value.Type : updateTo.Type;
                    p.Value.Notification = DateTime.Parse(p.Value.Expiration) < DateTime.Now
                        ? Product.ExpiredMessage
                        : Product.FreshMessage;
                }
                return true;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return false;
            }
        }

        private static void CheckProductsForExpiration(object source, ElapsedEventArgs args)
        {
            var expiredItems = GetAllExpired();

            foreach (var item in expiredItems)
            {
                var message = string.Format("Product with label {0} expired.", item.Label);
                //if the message has not been sent yet for that item, send it
                if (MsgsSent.All(m => m != message))
                {
                    MsgsSent.Add(message);
                    Notification.SendMessage(message);
                }
            }
        }

        private static IEnumerable<Product> GetAllExpired()
        {
            return Data.Values.Where(d => DateTime.Parse(d.Expiration) < DateTime.Now);
        }

    }
}