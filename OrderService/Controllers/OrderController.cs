using Microsoft.AspNetCore.Mvc;
using Responses.OrderService;

namespace OrderService.Controllers
{
    [ApiController]
    [Route("/api/orderservice/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly ILogger<OrderController> _logger;

        public OrderController(ILogger<OrderController> logger)
        {
            _logger = logger;
        }

        [HttpGet, Route("GeAllOrders")]
        public IEnumerable<GetAllOrderResponse> Get()
        {
            return Enumerable.Range(1, 5).Select(index => new GetAllOrderResponse
            {
                Id = index,
                OrderCode = "OD0001",
                CreateDate = DateTime.Now,
                ProductId = index,
            })
            .ToArray();
        }

        [HttpGet, Route("Tom")]
        public TomeResponse GetTom()
        {
            return new TomeResponse { Age = 10 };
        }
    }
}
