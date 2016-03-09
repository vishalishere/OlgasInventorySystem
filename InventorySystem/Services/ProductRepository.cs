using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using InventorySystem.Controllers;
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
            expirationChecker.StartTimer();
        }

        public IEnumerable<Product> GetAllProducts()
        {
            return Data.Values;
        }

        public void SaveProduct(IEnumerable<Product> productsToSave)
        {
            var failedProducts = new List<string>();
            foreach (var product in productsToSave)
            {
                product.Notification = DateTime.Parse(product.Expiration) < DateTime.Now ? Product.ExpiredMessage : Product.FreshMessage;
                if (!Data.TryAdd(product.Label, product))
                    failedProducts.Add(product.Label);
            }

            if (failedProducts.Count > 0)
            {
                var unsuccessfulProducts = failedProducts.Aggregate(string.Empty, (current, failedProduct) => current + failedProduct + ", ");
                unsuccessfulProducts = unsuccessfulProducts.Remove(unsuccessfulProducts.Length - 2);

                throw new KnownException(string.Format("Products with labels {0} could not be saved.", unsuccessfulProducts));
            }

        }

        public Product GetProductByLabel(string label)
        {
            if(Data.ContainsKey(label))
                return Data[label];
            return null;
        }

        public void RemoveProduct(string label)
        {
            Product product;

            if (Data.TryRemove(label, out product))
                Notification.SendMessage(string.Format("Product with label {0} was deleted.", label));
            else
                throw new KnownException(string.Format("Product with label {0} could not be removed", label));
        }

        public void RemoveAllProducts()
        {
            Data.Clear();
        }

        public void UpdateProduct(string label, Product updateTo)
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
            }
            catch(Exception e)
            {
                throw new KnownException(string.Format("Product with label {0} could not be updated", label));
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