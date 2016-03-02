using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Hosting;
using InventorySystem.Models;
using InventorySystem.Services;
using Newtonsoft.Json;
using SignalR;

namespace InventorySystem.Controllers
{
    public class ProductController : ApiController
    {
        private readonly IProductRepository _productRepository;

        public ProductController()
            : this(new ProductRepository(HttpContext.Current, true))
        {
        }

        public ProductController(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        // GET api/products
        public HttpResponseMessage Get()
        {
            var products = _productRepository.GetAllProducts();
            CheckRequestIsNotNull();

            return Request.CreateResponse(HttpStatusCode.OK, products);

        }

        // GET api/products/Milk
        public HttpResponseMessage Get(string id)
        {
            var products = _productRepository.GetProductByLabel(id);

            CheckRequestIsNotNull();

            var response = products.ToList().Count == 0 ? Request.CreateResponse(HttpStatusCode.NotFound, "The item could not be found.") : Request.CreateResponse(HttpStatusCode.OK, products);
            
            return response;
        }

        // POST api/products
        public HttpResponseMessage Post(HttpRequestMessage productsMsg)
        {
            HttpContent requestContent = productsMsg.Content;
            string jsonContent = requestContent.ReadAsStringAsync().Result;

            IEnumerable<Product> products = JsonConvert.DeserializeObject<IEnumerable<Product>>(jsonContent);
            var result = _productRepository.SaveProduct(products);
            CheckRequestIsNotNull();
            var resultMsg = result ? Request.CreateResponse(HttpStatusCode.Created, products) : Request.CreateResponse(HttpStatusCode.InternalServerError, string.Format("Product {0} could not be added.", productsMsg));
            
            return resultMsg;
        }

        // PUT api/products/milk
        public HttpResponseMessage Put(string id, Product value)
        {
            HttpResponseMessage response;
            CheckRequestIsNotNull();

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
            CheckRequestIsNotNull();

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
        
        private void CheckRequestIsNotNull()
        {
            if (Request == null)
            {
                Request = new HttpRequestMessage();
                Request.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration());
            }
        }
    }

    public class NotificationController : ApiController
    {
        private readonly IProductRepository _productRepository;

        public NotificationController()
            : this(new ProductRepository(HttpContext.Current, true))
        { }

        public NotificationController(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public HttpResponseMessage Get()
        {
            var result = _productRepository.GetAllMsgsSent();

            return Request.CreateResponse(HttpStatusCode.OK, result);
        }

    }

}
