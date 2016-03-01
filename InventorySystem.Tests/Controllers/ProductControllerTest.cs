using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using InventorySystem.Controllers;
using InventorySystem.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace InventorySystem.Tests.Controllers
{
    [TestClass]
    public class ProductControllerTest
    {
        readonly FakeProductRepository _productRepository = new FakeProductRepository();
        private readonly Product[] _products = {
                        new Product("Milk", "Drink", new DateTime(2017, 11, 5)),
                        new Product("Orange Juice", "Drink", new DateTime(2014, 2, 3))};

        private readonly List<Product> _alcoholicBeverages = new List<Product> { new Product("Wine", "Drink", new DateTime(2017, 10, 5)) };
        private readonly Product _updateTo = new Product("Milk", "Dairy", new DateTime(2017, 2, 2));

        [TestMethod]
        public void Get()
        {
            // Arrange
            var controller = new ProductController(_productRepository);

            // Act
            var result = controller.Get();

            HttpContent requestContent = result.Content;
            string jsonContent = requestContent.ReadAsStringAsync().Result;
            IEnumerable<Product> resultingProducts = JsonConvert.DeserializeObject<IEnumerable<Product>>(jsonContent);

            // Assert
            Assert.IsTrue(result.IsSuccessStatusCode);

            //Checks message
            Assert.AreEqual(2, resultingProducts.Count());
            Assert.IsTrue(_products[0].Equals(resultingProducts.ElementAt(0)));
            Assert.IsTrue(_products[1].Equals(resultingProducts.ElementAt(1)));

            //Checks content
            Assert.AreEqual(2, _productRepository.Products.Count());
            Assert.IsTrue(_products[0].Equals(_productRepository.Products.ElementAt(0)));
            Assert.IsTrue(_products[1].Equals(_productRepository.Products.ElementAt(1)));

        }

        [TestMethod]
        public void GetById()
        {
            // Arrange
            var controller = new ProductController(_productRepository);

            // Act
            var result = controller.Get("Milk");
            HttpContent requestContent = result.Content;
            string jsonContent = requestContent.ReadAsStringAsync().Result;
            IEnumerable<Product> resultingProducts = JsonConvert.DeserializeObject<IEnumerable<Product>>(jsonContent);

            // Assert
            Assert.IsTrue(result.IsSuccessStatusCode);

            // Checks message
            Assert.IsTrue(_products[0].Equals(resultingProducts.ElementAt(0)));

            //Checks content
            Assert.IsTrue(_products[0].Equals(_productRepository.Products.ElementAt(0)));

        }

        [TestMethod]
        public void Post()
        {
            // Arrange
            var controller = new ProductController(_productRepository);

            // Act
            var toPost = new HttpRequestMessage()
            {
                Content = new ObjectContent<List<Product>>(_alcoholicBeverages, new JsonMediaTypeFormatter())
            };

            var result = controller.Post(toPost);
            HttpContent requestContent = result.Content;
            string jsonContent = requestContent.ReadAsStringAsync().Result;
            IEnumerable<Product> resultingProducts = JsonConvert.DeserializeObject<IEnumerable<Product>>(jsonContent);

            // Assert
            Assert.IsTrue(result.IsSuccessStatusCode);

            //Checks message
            Assert.IsTrue(resultingProducts.Any(x => x.Equals(_alcoholicBeverages.First())));

            //Checks content
            Assert.AreEqual(3, _productRepository.Products.Count());
            Assert.IsTrue(_productRepository.Products.Any(x => x.Equals(_alcoholicBeverages.First())));

        }

        [TestMethod]
        public void Put()
        {
            // Arrange
            var controller = new ProductController(_productRepository);

            // Act
            var result = controller.Put("milk", _updateTo);

            // Assert
            Assert.IsTrue(result.IsSuccessStatusCode);

            //Checks contents
            Assert.IsTrue(_productRepository.Products.Any(x => x.Equals(_updateTo)));
        }

        [TestMethod]
        public void Delete()
        {
            // Arrange
            var controller = new ProductController(_productRepository);

            // Act
            var result = controller.Delete("milk");

            // Assert
            Assert.IsTrue(result.IsSuccessStatusCode);

            //Checks contents
            Assert.IsFalse(_productRepository.Products.Any(x => x.Label.ToLower() == "milk"));
        }
    }
}
