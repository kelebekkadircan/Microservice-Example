﻿using MassTransit;
using ORDER.API.Models.Entities;
using ORDER.API.Models.Enums;
using ORDER.API.Models;
using Shared.Events;
using Microsoft.EntityFrameworkCore;

namespace ORDER.API.Consumer
{
    public class PaymentFailedEventConsumer : IConsumer<PaymentFailedEvent>
    {
        readonly OrderAPIDbContext _dbContext;

        public PaymentFailedEventConsumer(OrderAPIDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Consume(ConsumeContext<PaymentFailedEvent> context)
        {
            Order order = await _dbContext.Orders.FirstOrDefaultAsync(o => o.OrderID == context.Message.OrderID);
            order.OrderStatus = OrderStatus.Failed;
            await _dbContext.SaveChangesAsync();
        }
    }
}
