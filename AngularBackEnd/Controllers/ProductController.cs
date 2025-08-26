using AngularBackEnd.Models.CustomerManagement;
using AngularBackEnd.Services.ProductManagement;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace AngularBackEnd.Controllers
{
    [EnableCors("AllowOrigin")]
    [Route("api/products")]
    [ApiController]

    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        // GET: api/products
        [HttpGet]
        public ActionResult<List<Product>> GetAll()
        {
            var products = _productService.GetAll();
            return Ok(products);
        }

        // GET: api/products/{id}
        [HttpGet("{id}")]
        public ActionResult<Product> GetById(int id)
        {
            var product = _productService.GetById(id);
            if (product == null)
                return NotFound();

            return Ok(product);
        }

        // POST: api/products
        [HttpPost]
        public IActionResult Add([FromBody] Product product)
        {
            _productService.Add(product);
            return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
        }

        // PUT: api/products/{id}
        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] Product product)
        {
            var existing = _productService.GetById(id);
            if (existing == null)
                return NotFound();

            product.Id = id;
            _productService.Update(product);
            return NoContent();
        }

        // DELETE: api/products/{id}
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var existing = _productService.GetById(id);
            if (existing == null)
                return NotFound();

            _productService.Delete(id);
            return NoContent();
        }
    }
}

