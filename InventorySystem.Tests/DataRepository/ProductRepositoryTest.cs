using System.Collections.Generic;
using System.Linq;
using InventorySystem.Models;
using InventorySystem.Services;
using NUnit.Framework;

namespace InventorySystem.Tests.DataRepository
{
    [TestFixture]
    public class ProductRepositoryTest
    {
        private readonly ProductRepository _productRepository = new ProductRepository();
        private readonly List<Product> _products = new List<Product>() {new Product("Milk", "Drink", "11/1/2016"), new Product("Chips", "Snack", "12/1/2013")};

        [Test]
        public void GetAllProducts()
        {
            Assert.IsEmpty(_productRepository.GetAllProducts());
        }

        [Test]
        public void SaveProduct()
        {
            Assert.IsEmpty(_productRepository.GetAllProducts());

            _productRepository.SaveProduct(_products);
            var products = _productRepository.GetAllProducts();
            Assert.That(products.Count() == 2);
            Assert.That(products.ElementAt(0).Equals(_products[0]));
            Assert.That(products.ElementAt(1).Equals(_products[1]));

            _productRepository.RemoveAllProducts();
        }

        [Test]
        public void GetProductByLabel()
        {
            Assert.IsEmpty(_productRepository.GetAllProducts());

            _productRepository.SaveProduct(_products);
            var product =_productRepository.GetProductByLabel("Milk");
            Assert.That(product.Equals(_products.ElementAt(0)));

            _productRepository.RemoveAllProducts();
        }

        [Test]
        public void RemoveProduct()
        {
            Assert.IsEmpty(_productRepository.GetAllProducts());

            _productRepository.SaveProduct(_products);
            var products = _productRepository.GetAllProducts();
            Assert.That(products.Count() == 2);

            _productRepository.RemoveProduct("Milk");
            products = _productRepository.GetAllProducts();

            Assert.That(products.Count() == 1);
            Assert.False(products.Any(p=>p.Label == "Milk"));

            _productRepository.RemoveAllProducts();
        }

        [Test]
        public void UpdateProduct()
        {
            Assert.IsEmpty(_productRepository.GetAllProducts());
            _productRepository.SaveProduct(_products);
            var products = _productRepository.GetAllProducts();
            Assert.That(products.ElementAt(1).Label == "Chips");
            Assert.That(products.ElementAt(1).Type == "Snack");

            var productToUpdateTo = new Product("Chips", "Junk", "2/3/2014");
            _productRepository.UpdateProduct("Chips", productToUpdateTo);
            Assert.That(_productRepository.GetProductByLabel("Chips").Equals(productToUpdateTo));

            _productRepository.RemoveAllProducts();
        }

    }
}
