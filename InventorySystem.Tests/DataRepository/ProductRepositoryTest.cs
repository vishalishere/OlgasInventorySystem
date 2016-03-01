using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Web;
using InventorySystem.Models;
using InventorySystem.Services;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace InventorySystem.Tests.DataRepository
{
    [TestClass]
    public class ProductRepositoryTest
    {
        private readonly List<Product> _alcoholicBeverages = new List<Product> { new Product("Wine", "Drink", new DateTime(2017, 10, 5)) };
        private readonly Product _updateTo = new Product("Milk", "Dairy", new DateTime(2017, 2, 2));

        [TestMethod]
        public void GetAllProducts()
        {
            ProductRepository productRepository = GetProductRepository();
            Assert.IsTrue(productRepository.GetAllProducts().Count() == 2);
        }

        [TestMethod]
        public void SaveProduct()
        {
            ProductRepository productRepository = GetProductRepository();
            var intialData = productRepository.GetAllData();

            Assert.IsFalse(intialData.Any(x => x.Equals(_alcoholicBeverages.First())));

            productRepository.SaveProduct(_alcoholicBeverages);
            var result = productRepository.GetAllData();

            Assert.IsTrue(result.Any(x => x.Equals(_alcoholicBeverages.First())));
        }

        [TestMethod]
        public void GetProductByLabel()
        {
            ProductRepository productRepository = GetProductRepository();
            var result = productRepository.GetProductByLabel("Milk");
            Assert.IsTrue(result.First().Equals(new Product("Milk", "Drink", new DateTime(2017, 11, 5))));

        }

        [TestMethod]
        public void RemoveProduct()
        {
            ProductRepository productRepository = GetProductRepository();
            var initialData = productRepository.GetAllData();
            Assert.IsTrue(initialData.Any(x => x.Label == "Milk"));

            productRepository.RemoveProduct("Milk");
            var result = productRepository.GetAllData();

            Assert.IsFalse(result.Any(x => x.Label == "Milk"));
        }

        [TestMethod]
        public void UpdateProduct()
        {
            ProductRepository productRepository = GetProductRepository();
            var dataBeforeUpdate = productRepository.GetAllData();
            Assert.IsFalse(dataBeforeUpdate.Any(x => x.Equals(_updateTo)));

            productRepository.UpdateProduct("milk", _updateTo);
            var dataAfterUpdate = productRepository.GetAllData();
            Assert.IsTrue(dataAfterUpdate.Any(x => x.Equals(_updateTo)));
        }

        [TestMethod]
        public void Notifications()
        {
            //use moq to checks whether clients are recieving the notifications
        }

        private static ProductRepository GetProductRepository()
        {
            HttpContext.Current = new HttpContext(
                new HttpRequest("", "http://localhost:57016", ""),
                new HttpResponse(new StringWriter())
                );
            return new ProductRepository(HttpContext.Current, false);
        }
    }
}
