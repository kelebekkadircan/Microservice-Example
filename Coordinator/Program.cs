using Coordinator.Models.Contexts;
using Coordinator.Services.Abstraction;
using Coordinator.Services.Concrete;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<TwoPhaseCommitContext>(opt =>
{
    opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});
builder.Services.AddHttpClient("OrderAPI", client =>
{
    client.BaseAddress = new Uri("https://localhost:7091");
});
builder.Services.AddHttpClient("StockAPI", client =>
{
    client.BaseAddress = new Uri("https://localhost:7189");
});

builder.Services.AddHttpClient("PaymentAPI", client =>
{
    client.BaseAddress = new Uri("https://localhost:7093");
});

//builder.Services.AddSingleton<ITransactionService, TransactionService>();
builder.Services.AddScoped<ITransactionService, TransactionService>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();

//app.UseAuthorization();

//app.MapControllers();

app.MapGet("/create-order-transaction", async (ITransactionService transactionService) =>
{
    // phase 1 prepare
    Guid transactionID = await transactionService.CreateTransactionAsync();
    await transactionService.PrepareServicesAsync(transactionID);
    bool transactionState = await transactionService.CheckReadyServicesAsync(transactionID);

    if (transactionState)
    {
        // phase 2 Commit
        await transactionService.CommitAsync(transactionID);
        transactionState =  await transactionService.CheckTransactionStateServicesAsync(transactionID);


    }
    if(!transactionState)
    {
        // phase 2 Rollback
        await transactionService.RollbackAsync(transactionID);
    }

});

app.Run();
