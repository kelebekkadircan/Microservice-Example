using MassTransit;
using MongoDB.Driver;
using Shared;
using Shared.Events;
using Shared.Messages;
using STOCK.API.Models.Entities;
using STOCK.API.Services;

namespace STOCK.API.Consumers
{
    public class OrderCreatedEventConsumer : IConsumer<OrderCreatedEvent>
    {
        IMongoCollection<Stock> _stockColleciton;
        readonly ISendEndpointProvider _sendEndpointProvider;
        readonly IPublishEndpoint _publishEndpoint;

        public OrderCreatedEventConsumer(MongoDBService mongoDBService, ISendEndpointProvider sendEndpointProvider, IPublishEndpoint publishEndpoint)
        {
            _stockColleciton = mongoDBService.GetCollection<Stock>();
            _sendEndpointProvider = sendEndpointProvider;
            _publishEndpoint = publishEndpoint;
        }

        public async Task Consume(ConsumeContext<OrderCreatedEvent> context)
        {
            List<bool> stockResult = new();
            foreach (OrderItemMessage item in context?.Message?.OrderItemMessages)
            {
              stockResult.Add(( await _stockColleciton.FindAsync(s => s.ProductID == item.ProductID && s.Count >= item.Count)).Any());
            }

            if(stockResult.TrueForAll(sr => sr.Equals(true)))
            {
                foreach(var item in context.Message.OrderItemMessages)
                {
                    Stock stock = await (await _stockColleciton.FindAsync(x => x.ProductID == item.ProductID)).FirstOrDefaultAsync();
                    
                    stock.Count -= item.Count;

                    await _stockColleciton.FindOneAndReplaceAsync(x => x.ProductID == item.ProductID, stock);

                }

                StockReservedEvents stockReservedEvent = new()
                {
                    BuyerID = context.Message.BuyerID,
                    OrderID = context.Message.OrderID,
                    TotalPrice = context.Message.TotalPrice,

                };

                ISendEndpoint sendEndpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri($"queue:{RabbitMQSettings.Payment_StockReservedEventQueue}"));

                await sendEndpoint.Send(stockReservedEvent);

            }
            else
            {
                // sipariş geçersiz...
                StockNotReservedEvent stockNotReservedEvent = new()
                {
                    OrderID = context.Message.OrderID,
                    BuyerID = context.Message.BuyerID,
                    Message = "Stock not reserved"
                };

                await _publishEndpoint.Publish(stockNotReservedEvent);


            }
                



        }
    }
}
