using BrotliSharpLib;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Ocelot.Middleware;
using Ocelot.Multiplexer;
using Responses.OrderService;
using Responses.ProductService;
using System.Net;
using System.Net.Http.Headers;
using System.Text.Json.Serialization.Metadata;

namespace ApiGateway.Aggregators
{
    /// <summary>
    /// Reference: https://github.com/ThreeMammals/Ocelot/issues/1207
    /// </summary>
    public class FakeDefinedAggregator : IDefinedAggregator
    {
        public async Task<DownstreamResponse> Aggregate(List<HttpContext> responseHttpContexts)
        {
            try
            {
                var xResponseContent = await responseHttpContexts.FirstOrDefault(x => x.Items.DownstreamRoute().Key == "GeAllProducts").Items.DownstreamResponse().Content.ReadAsStringAsync();
                var yResponseContent = await responseHttpContexts.FirstOrDefault(x => x.Items.DownstreamRoute().Key == "GeAllOrders").Items.DownstreamResponse().Content.ReadAsStringAsync();
                
                List<GetAllProductResponse> products = JsonConvert.DeserializeObject<List<GetAllProductResponse>>(xResponseContent);
                List<GetAllOrderResponse> orders = JsonConvert.DeserializeObject<List<GetAllOrderResponse>>(yResponseContent);

                List<JObject> ordersWithProductName = new List<JObject>();
                foreach (var order in orders)
                {
                    var productName = products?.Find(p => p.Id == order.ProductId)?.Name;
                    var orderJObject = JObject.FromObject(order);
                    orderJObject.Add(new JProperty("ProductName", productName));
                    ordersWithProductName.Add(orderJObject);
                   
                }
                var ordersWithProductNameString = JsonConvert.SerializeObject(ordersWithProductName);
                var stringContent = new StringContent(ordersWithProductNameString)
                {
                    Headers = { ContentType = new MediaTypeHeaderValue("application/json") }
                };

                return new DownstreamResponse(stringContent, HttpStatusCode.OK, new List<KeyValuePair<string, IEnumerable<string>>>(), "OK");
            }
            catch (Exception ex)
            {

                throw;
            }

            // The aggregator gets a list of downstream responses as parameter.
            // You can now implement your own logic to aggregate the responses (including bodies and headers) from the downstream services
            var responses = responseHttpContexts.Select(x => x.Items.DownstreamResponse()).ToArray();

            // In this example we are concatenating the results,
            // but you could create a more complex construct, up to you.
            var contentList = new List<string>();
            foreach (var response in responses)
            {
                var content = await response.Content.ReadAsStringAsync();
                contentList.Add(content);
            }

            // The only constraint here: You must return a DownstreamResponse object.
            return new DownstreamResponse(
                //new StringContent(JsonConvert.SerializeObject(contentList)),
                JsonContent.Create(contentList),
                HttpStatusCode.OK,
                responses.SelectMany(x => x.Headers).ToList(),
                "reason");
        }

        private string DeCompressBrotli(byte[] xResponseContent)
        {
            return System.Text.Encoding.UTF8.GetString(Brotli.DecompressBuffer(xResponseContent, 0, xResponseContent.Length, null));
        }
    }
}

