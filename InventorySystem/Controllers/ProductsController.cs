using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using InventorySystem.Models;
using InventorySystem.Services;
using Newtonsoft.Json;

namespace InventorySystem.Controllers
{
    public class ProductsController : ApiController
    {
        private readonly ProductRepository _productRepository = new ProductRepository();

        // GET api/product
        public HttpResponseMessage Get()
        {
            return Request.CreateResponse(HttpStatusCode.OK, _productRepository.GetAllProducts());
        }

        // POST api/products
        public HttpResponseMessage Post(HttpRequestMessage productsMsg)
        {
            HttpContent requestContent = productsMsg.Content;
            string jsonContent = requestContent.ReadAsStringAsync().Result;
            IEnumerable<Product> products = JsonConvert.DeserializeObject<IEnumerable<Product>>(jsonContent);

            var result = _productRepository.SaveProduct(products);

            return result ? Request.CreateResponse(HttpStatusCode.Created, products) : Request.CreateResponse(HttpStatusCode.InternalServerError, string.Format("Product {0} could not be added.", productsMsg));
        }

    }
}
