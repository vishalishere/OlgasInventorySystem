using System.Collections.Generic;
using System.Net.Http;
using InventorySystem.Models;

namespace InventorySystem.Services
{
    public interface IProductRepository
    {
        IEnumerable<Product> GetAllProducts();
        bool SaveProduct(IEnumerable<Product> productsToSave);
        IEnumerable<Product> GetProductByLabel(string label);
        bool RemoveProduct(string label);
        bool UpdateProduct(string label, Product updateTo);
    }
}