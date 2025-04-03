using MassTransit;
using Microsoft.EntityFrameworkCore;
using ORDER.API.Models;
using ORDER.API.Models.Entities;
using ORDER.API.Models.Enums;
using Shared.Events;

namespace ORDER.API.Consumer
{
    public class PaymentCompletedEventConsumer : IConsumer<PaymentCompletedEvent>
    {
        readonly OrderAPIDbContext _dbContext;

        public PaymentCompletedEventConsumer(OrderAPIDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Consume(ConsumeContext<PaymentCompletedEvent> context)
        {
            Order order = await _dbContext.Orders.FirstOrDefaultAsync(o => o.OrderID == context.Message.OrderID);
            order.OrderStatus = OrderStatus.Completed;
            await _dbContext.SaveChangesAsync();
        }
    }
}
