using System;
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

            try
            {
                _productRepository.SaveProduct(products);
                return Request.CreateResponse(HttpStatusCode.Created, products);
            }
            catch (KnownException e)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, e.ErrorMessage);
            }
            catch (Exception e)
            {
                //logger.Log(e)
                return Request.CreateResponse(HttpStatusCode.InternalServerError,
                    "Something went wrong.");
            }
        }

        // PUT api/products/milk
        public HttpResponseMessage Put(string id, Product value)
        {
            var msg = Get(id);

            if (msg.StatusCode == HttpStatusCode.NotFound)
                return msg;
            try
            {
                _productRepository.UpdateProduct(id, value);
                return Request.CreateResponse(HttpStatusCode.OK, string.Format("Products with label {0} has been updated.", id));

            }
            catch(KnownException e)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, e.ErrorMessage);
            }
            catch (Exception e)
            {
                //logger.Log(e)
                return Request.CreateResponse(HttpStatusCode.InternalServerError,
                    "Something went wrong.");
            }
            
        }

        // DELETE api/products/milk
        public HttpResponseMessage Delete(string id)
        {
            var msg = Get(id);

            if (msg.StatusCode == HttpStatusCode.NotFound)
                return msg;
            try
            {
                _productRepository.RemoveProduct(id);
                return Request.CreateResponse(HttpStatusCode.OK, string.Format("Products with label {0} have been deleted.", id));
            }
            catch(KnownException e)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, e.ErrorMessage);
            }
            catch (Exception e)
            {
                //logger.Log(e)
                return Request.CreateResponse(HttpStatusCode.InternalServerError,
                    "Something went wrong.");
            }
        }
    }
}
