using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Hosting;
using InventorySystem.Models;
using InventorySystem.Services;
using Newtonsoft.Json;

namespace InventorySystem.Controllers
{
    public class ProductsController : ApiController
    {
        private readonly IProductRepository _productRepository;

        public ProductsController()
            : this(new ProductRepository(HttpContext.Current, true))
        {
        }

        public ProductsController(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        // GET api/product
        public HttpResponseMessage Get()
        {
            var products = _productRepository.GetAllProducts();
            CheckRequestIsNotNull();

            return Request.CreateResponse(HttpStatusCode.OK, products);

        }

        // POST api/products
        public HttpResponseMessage Post(HttpRequestMessage productsMsg)
        {
            HttpContent requestContent = productsMsg.Content;
            string jsonContent = requestContent.ReadAsStringAsync().Result;
            IEnumerable<Product> products = JsonConvert.DeserializeObject<IEnumerable<Product>>(jsonContent);

            var result = _productRepository.SaveProduct(products);
            CheckRequestIsNotNull();

            return result ? Request.CreateResponse(HttpStatusCode.Created, products) : Request.CreateResponse(HttpStatusCode.InternalServerError, string.Format("Product {0} could not be added.", productsMsg));
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
}
