using DapperCrudAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace DapperCrudAPI.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly ProductRepository productRepository;

        public ProductController(ProductRepository productRepository)
        {
            this.productRepository = productRepository;
        }

        [HttpGet]
        public IEnumerable<Product> Get()
        {
            return productRepository.GetAll();
        }

        [HttpGet("{id}")]
        public Product GetProduct(int id)
        {
            return productRepository.GetById(id);
        }

        [HttpPost]
        public void Post(Product product)
        {
            if (ModelState.IsValid)
                productRepository.Add(product);
        }

        [HttpPut("{id}")]
        public void Put(int id, Product product)
        {
            product.Id = id;
            if (ModelState.IsValid)
                productRepository.Update(product);
        }

        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            productRepository.Delete(id);
        }
    }

}