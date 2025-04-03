using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ORDER.API.Models;
using ORDER.API.Models.Entities;
using ORDER.API.Models.Enums;
using ORDER.API.ViewModels;
using Shared.Events;
using Shared.Messages;

namespace ORDER.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {

        readonly OrderAPIDbContext _dbContext;
        readonly IPublishEndpoint _publishEndpoint;

        public OrdersController(OrderAPIDbContext dbContext, IPublishEndpoint publishEndpoint)
        {
            _dbContext = dbContext;
            _publishEndpoint = publishEndpoint;
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder(CreateOrderVM createOrder)
        {
            Order order = new Order
            {
                OrderID = Guid.NewGuid(),
                CreatedDate = DateTime.Now,
                OrderStatus = OrderStatus.Suspended,
                BuyerID = createOrder.BuyerID,
                
            };

            order.OrderItems = createOrder?.OrderItems?.Select(x => new OrderItem
            {
                OrderItemID = Guid.NewGuid(),
                OrderID = order.OrderID,
                ProductID = x.ProductID,
                Count = x.Count,
                Price = x.Price
            }).ToList();

            order.TotalPrice = order.OrderItems.Sum(x => x.Price * x.Count);

            await _dbContext.Orders.AddAsync(order);
            await _dbContext.SaveChangesAsync();

            OrderCreatedEvent orderCreatedEvent = new()
            {
                BuyerID = order.BuyerID,
                OrderID = order.OrderID,
                OrderItemMessages = order.OrderItems.Select(x => new OrderItemMessage
                {
                    Count = x.Count,
                    ProductID = x.ProductID,    

                }).ToList(),
                TotalPrice = order.TotalPrice

            };

            await _publishEndpoint.Publish(orderCreatedEvent);

            return Ok();
        }
    }
}
