using Microsoft.AspNetCore.Mvc;
using Responses.ProductService;

namespace ProductService.Controllers
{
    [ApiController]
    [Route("/api/productservice/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly ILogger<ProductController> _logger;

        public ProductController(ILogger<ProductController> logger)
        {
            _logger = logger;
        }

        [HttpGet, Route("GeAllProducts")]
        public IEnumerable<GetAllProductResponse> Get()
        {
            return Enumerable.Range(1, 5).Select(index => new GetAllProductResponse
            {
                Id = index,
                Name = "Macbook Pro 2024",
            })
            .ToArray();
        }

        [HttpGet, Route("Laura")]
        
        public LauraResponse GetTom()
        {
            return new LauraResponse { Age = 20 };
        }
    }
}
