using Microsoft.AspNetCore.Mvc;
using RealWordUnitTest.Web.Models;
using RealWordUnitTest.Web.Repository;

namespace RealWordUnitTest.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsApiController : ControllerBase
    {
        private readonly IRepository<Product> _productRepository;

        public ProductsApiController(IRepository<Product> productRepository)
        {
            _productRepository = productRepository;
        }

        // GET: api/ProductsApi
        [HttpGet]
        public async Task<IActionResult> GetProducts()
        {
            var products = await _productRepository.GetAll();

            return Ok(products);
        }

        // GET: api/ProductsApi/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProduct(int id)
        {
            var product = await _productRepository.GetById(id);

            if (product == null)
            {
                return NotFound();
            }

            return Ok(product);
        }

        // PUT: api/ProductsApi/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public IActionResult PutProduct(int id, Product product)
        {
            if (id != product.Id)
            {
                return BadRequest();
            }

            _productRepository.Update(product);

            return NoContent();
        }

        // POST: api/ProductsApi
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<IActionResult> PostProduct(Product product)
        {
            await _productRepository.Create(product);

            return CreatedAtAction("GetProduct", new { id = product.Id }, product);
        }

        // DELETE: api/ProductsApi/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _productRepository.GetById(id);

            if (product == null)
            {
                return NotFound();
            }

            _productRepository.Delete(product);

            return NoContent();
        }

        private bool ProductExists(int id)
        {
            var product = _productRepository.GetById(id).Result;

            return product == null;
        }
    }
}
