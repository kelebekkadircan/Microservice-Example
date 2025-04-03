using MassTransit;
using Shared.Events;

namespace PAYMENT.API.Consumers
{
    public class StockReservedEventConsumers : IConsumer<StockReservedEvents>
    {
        IPublishEndpoint _publishEndpoint;

        public StockReservedEventConsumers(IPublishEndpoint publishEndpoint)
        {
            _publishEndpoint = publishEndpoint;
        }

        public async Task Consume(ConsumeContext<StockReservedEvents> context)
        {
            // ödeme işlemleri

            if(true)
            {
                // ödeme işlemi başarılı...
                PaymentCompletedEvent paymentCompletedEvent = new()
                {
                    OrderID = context.Message.OrderID
                };

                await _publishEndpoint.Publish(paymentCompletedEvent);


            }
            else
            {
                // ödeme işlemi başarısız...
                PaymentFailedEvent paymentFailedEvent = new()
                {
                    OrderID = context.Message.OrderID,
                    Message = "Payment failed"
                };
               await _publishEndpoint.Publish(paymentFailedEvent);

            }

        }
    }
}
