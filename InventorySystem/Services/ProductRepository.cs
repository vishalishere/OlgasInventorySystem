﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using System.Web;
using InventorySystem.Models;
using Microsoft.AspNet.SignalR;

namespace InventorySystem.Services
{
    public class ProductRepository : IProductRepository
    {
        public HttpContext Ctx;
        public string CacheKey = "ProductStore";
        private readonly Timer _timer = new Timer();
        private readonly double _secondsInADay = 86400;

        public ProductRepository(HttpContext ctx, bool rememberState)
        {
            Ctx = ctx;
            if (Ctx != null)
            {
                _timer.Elapsed += SendMessage;
                _timer.Interval = _secondsInADay;
                _timer.Start();

                if (Ctx.Cache[CacheKey] == null || !rememberState)
                {
                    var products = new[]
                    {
                        new Product("Milk", "Drink", new DateTime(2017, 11, 5)),
                        new Product("Orange Juice", "Drink", new DateTime(2014, 2, 3))
                    };

                    Ctx.Cache[CacheKey] = products;
                }
            }
        }

        public IEnumerable<Product> GetAllProducts()
        {
            if (Ctx != null)
            {
                return (Product[])Ctx.Cache[CacheKey];
            }
            throw new Exception("In memory data object not found");
        }

        public bool SaveProduct(IEnumerable<Product> productsToSave)
        {
            if (Ctx != null)
            {
                try
                {
                    var currentData = ((Product[])Ctx.Cache[CacheKey]).ToList();
                    List<Product> productsToAdd = new List<Product>();
                    foreach (var product in productsToSave)
                    {
                        productsToAdd.Add(new Product(product.Label, product.Type, DateTime.Parse(product.Expiration)));
                    }

                    currentData.AddRange(productsToAdd);
                    Ctx.Cache[CacheKey] = currentData.ToArray();

                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    return false;
                }
            }
            return false;
        }

        public IEnumerable<Product> GetProductByLabel(string label)
        {
            var products = new List<Product>();

            if (Ctx != null)
            {
                try
                {
                    var currentData = ((Product[])Ctx.Cache[CacheKey]).ToList();
                    products.AddRange(currentData.Where(product => string.Equals(product.Label, label, StringComparison.CurrentCultureIgnoreCase)));
                    return products;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }

            }
            throw new Exception("In memory data object not found");
        }

        public bool RemoveProduct(string label)
        {
            if (Ctx != null)
            {
                try
                {
                    var currentData = ((Product[])Ctx.Cache[CacheKey]).ToList();
                    currentData.RemoveAll(product => string.Equals(product.Label, label, StringComparison.CurrentCultureIgnoreCase));
                    Ctx.Cache[CacheKey] = currentData.ToArray();
                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    return false;
                }
            }
            throw new Exception("In memory data object not found");
        }

        public bool UpdateProduct(string label, Product updateTo)
        {
            if (Ctx != null)
            {
                try
                {
                    var currentData = ((Product[])Ctx.Cache[CacheKey]).ToList();

                    foreach (var p in currentData.Where(product => string.Equals(product.Label, label, StringComparison.CurrentCultureIgnoreCase)))
                    {
                        p.Label = string.IsNullOrEmpty(updateTo.Label) ? p.Label : updateTo.Label;
                        p.Expiration = string.IsNullOrEmpty(updateTo.Expiration) ? p.Expiration : updateTo.Expiration;
                        p.Type = string.IsNullOrEmpty(updateTo.Type) ? p.Type : updateTo.Type;
                        p.Notification = DateTime.Parse(p.Expiration) < DateTime.Now ? "Product expired" : "Product is fresh";
                    }
                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    return false;
                }
            }
            throw new Exception("In memory data object not found");
        }

        private void SendMessage(object source, ElapsedEventArgs args)
        {
            var expiredItems = GetAllExpired();
            foreach (var item in expiredItems)
            {
                var message = string.Format("Item with label {0} expired.", item.Label);
                GlobalHost
                    .ConnectionManager
                    .GetHubContext<NotificationHub>().Clients.All.sendMessage(message);
            }
        }

        public Product[] GetAllData()
        {
            return (Product[])Ctx.Cache[CacheKey];
        }

        private IEnumerable<Product> GetAllExpired()
        {
            var allData = GetAllData();
            var expired = allData.Where(d => DateTime.Parse(d.Expiration) < DateTime.Now);
            return expired;
        }

    }
}