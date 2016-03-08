using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using InventorySystem.Models;
using InventorySystem.Services;
using Newtonsoft.Json;

namespace InventorySystem.Controllers
{
    public class ProductController : ApiController
    {
        private readonly ProductRepository _productRepository = new ProductRepository();

        // GET api/products
        public HttpResponseMessage Get()
        {
            return Request.CreateResponse(HttpStatusCode.OK, _productRepository.GetAllProducts());
        }

        // GET api/products/Milk
        public HttpResponseMessage Get(string id)
        {
            var product = _productRepository.GetProductByLabel(id);

            var response = product == null ? Request.CreateResponse(HttpStatusCode.NotFound, "The item could not be found.") : Request.CreateResponse(HttpStatusCode.OK, product);
            
            return response;
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

        // PUT api/products/milk
        public HttpResponseMessage Put(string id, Product value)
        {
            HttpResponseMessage response;

            var msg = Get(id);

            if (msg.StatusCode == HttpStatusCode.NotFound)
                response = msg;
            else if (_productRepository.UpdateProduct(id, value))
            {
                response = Request.CreateResponse(HttpStatusCode.OK, string.Format("Products with label {0} has been updated.", id));
            }
            else
            {
                response = Request.CreateResponse(HttpStatusCode.InternalServerError,
                    string.Format("Products with label {0} could not be updated.", id));
            }

            return response;
        }

        // DELETE api/products/milk
        public HttpResponseMessage Delete(string id)
        {
            HttpResponseMessage response;

            var msg = Get(id);

            if (msg.StatusCode == HttpStatusCode.NotFound)
                response = msg;
            else if (_productRepository.RemoveProduct(id))
            {
                response = Request.CreateResponse(HttpStatusCode.OK, string.Format("Products with label {0} have been deleted.", id));
            }
            else
            {
                response = Request.CreateResponse(HttpStatusCode.InternalServerError,
                    string.Format("Products with label {0} could not be deleted.", id));
            }

            return response;
        }
    }
}
