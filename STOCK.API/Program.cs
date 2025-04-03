using MassTransit;
using MongoDB.Driver;
using Shared;
using STOCK.API.Consumers;
using STOCK.API.Models.Entities;
using STOCK.API.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMassTransit(configurator =>
{
    configurator.AddConsumer<OrderCreatedEventConsumer>();

    configurator.UsingRabbitMq((context, _config) =>
    {
        _config.Host(builder.Configuration.GetConnectionString("RabbitMQ"));
        _config.ReceiveEndpoint(RabbitMQSettings.Stock_OrderCreatedEventQueue, e => e.ConfigureConsumer<OrderCreatedEventConsumer>(context));
    });
});

builder.Services.AddSingleton<MongoDBService>();

#region Seedmongodbdata


using IServiceScope scope = builder.Services.BuildServiceProvider().CreateScope();
MongoDBService? mongoDBService = scope.ServiceProvider.GetService<MongoDBService>();
var collection = mongoDBService.GetCollection<Stock>();
if (!collection.FindSync(s => true).Any())
{
    await collection.InsertOneAsync(new() { ProductID = Guid.NewGuid(), Count = 32 });
    await collection.InsertOneAsync(new() { ProductID = Guid.NewGuid(), Count = 23 });
    await collection.InsertOneAsync(new() { ProductID = Guid.NewGuid(), Count = 54 });
    await collection.InsertOneAsync(new() { ProductID = Guid.NewGuid(), Count = 62 });
    await collection.InsertOneAsync(new() { ProductID = Guid.NewGuid(), Count = 46 });
}

#endregion


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
