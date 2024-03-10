namespace Responses.OrderService
{
    public class GetAllOrderResponse
    {
        public int Id { get; set; }
        public string OrderCode { get; set; }
        public DateTime CreateDate { get; set; }
        public int ProductId { get; set; }
    }

    public class TomeResponse
    {
        public int Age { get; set; }
    }
}
