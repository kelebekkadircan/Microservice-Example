using Coordinator.Models;
using Coordinator.Models.Contexts;
using Coordinator.Services.Abstraction;
using Microsoft.EntityFrameworkCore;

namespace Coordinator.Services.Concrete
{
    public class TransactionService : ITransactionService
    {

        private readonly TwoPhaseCommitContext _context;
        private readonly HttpClient _orderHttpClient;
        private readonly HttpClient _stockHttpClient;
        private readonly HttpClient _paymentHttpClient;

        public TransactionService(TwoPhaseCommitContext context, IHttpClientFactory httpClientFactory)
        {
             _context = context;
             _orderHttpClient = httpClientFactory.CreateClient("OrderAPI");
             _stockHttpClient = httpClientFactory.CreateClient("StockAPI");
             _paymentHttpClient = httpClientFactory.CreateClient("PaymentAPI");
        }


        public async Task<Guid> CreateTransactionAsync()
        {
           Guid transacitonID =  Guid.NewGuid();
            var nodes = await _context.Nodes.ToListAsync();
            nodes.ForEach(node => node.NodeStates = new List<NodeState>()
            {
                new(transacitonID)
                {
                    IsReady = Enums.ReadyType.Pending,
                    TransactionState = Enums.TransactionStateType.Pending


                }
            });
            await _context.SaveChangesAsync();
            return transacitonID;
        }
        public async Task PrepareServicesAsync(Guid transactionID)
        {
           var transactionNodes = await _context.NodeStates
                .Include(ns => ns.Node)
                .Where(ns => ns.TransactionID == transactionID)
                .ToListAsync();

            foreach (var transactionNode in transactionNodes)
            {
                try
                {
                    // Zaman aşımı için cancellation token tanımlanabilir (opsiyonel)
                    using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
                    var response = await (transactionNode.Node.Name switch
                    {
                        "ORDER.API" => _orderHttpClient.GetAsync("ready",cts.Token),
                        "STOCK.API" => _stockHttpClient.GetAsync("ready",cts.Token),
                        "PAYMENT.API" => _paymentHttpClient.GetAsync("ready", cts.Token),
                        _ => throw new Exception("Unknown service")
                    });
                    string content = await response.Content.ReadAsStringAsync();

                    if (bool.TryParse(content, out bool isReady))
                    {
                        transactionNode.IsReady = isReady
                            ? Enums.ReadyType.Ready
                            : Enums.ReadyType.NotReady;
                    }
                    else
                    {
                        transactionNode.IsReady = Enums.ReadyType.NotReady;
                        // Log: dönüş değeri bool değil
                    }

                    //bool result = bool.Parse(await response.Content.ReadAsStringAsync());
                    //if (result)
                    //{
                    //    transactionNode.IsReady = Enums.ReadyType.Ready;

                    //}
                    //else
                    //{
                    //    transactionNode.IsReady = Enums.ReadyType.NotReady;

                    //}

                }
                catch(Exception ex)
                {
                    transactionNode.IsReady = Enums.ReadyType.NotReady;
                    throw new Exception($"Error while preparing service {transactionNode.Node.Name}", ex);
                }

            }
                await _context.SaveChangesAsync();

        }
        public async Task<bool> CheckReadyServicesAsync(Guid transactionID)
        => (await  _context.NodeStates
            .Where(ns => ns.TransactionID == transactionID)
            .ToListAsync()).TrueForAll(ns => ns.IsReady == Enums.ReadyType.Ready);

        public async Task CommitAsync(Guid transactionID)
        {
         var transactionNodes =  await  _context.NodeStates
               .Where(ns => ns.TransactionID == transactionID)
               .Include(ns => ns.Node)
               .ToListAsync();

            foreach ( var transactionNode in transactionNodes)
            {
                try
                {
                    using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
                    var response = await (transactionNode.Node.Name switch
                    {
                        "ORDER.API" => _orderHttpClient.GetAsync("commit", cts.Token),
                        "STOCK.API" => _stockHttpClient.GetAsync("commit", cts.Token),
                        "PAYMENT.API" => _paymentHttpClient.GetAsync("commit", cts.Token),
                        _ => throw new Exception("Unknown service")
                    });

                    string content = await response.Content.ReadAsStringAsync();

                    if (bool.TryParse(content, out bool isCommitted))
                    {
                        transactionNode.TransactionState = isCommitted
                            ? Enums.TransactionStateType.Done
                            : Enums.TransactionStateType.Aborted;
                    }
                    else
                    {
                        transactionNode.TransactionState = Enums.TransactionStateType.Aborted;
                        // Log: dönüş değeri bool değil
                    }


                }
                catch(Exception ex)
                {
                    transactionNode.TransactionState = Enums.TransactionStateType.Aborted;
                    throw new Exception($"Error while committing service {transactionNode.Node.Name}", ex);
                }
                
            }
            await _context.SaveChangesAsync();

        }

        public async Task<bool> CheckTransactionStateServicesAsync(Guid transactionID)
        {
            return (await _context.NodeStates
                 .Where(ns => ns.TransactionID == transactionID)
                 .ToListAsync()).TrueForAll(ns => ns.TransactionState == Enums.TransactionStateType.Done);
        }




        public async Task RollbackAsync(Guid transactionID)
        {
            var transactionNodes = 
                await _context.NodeStates
                             .Where(ns => ns.TransactionID == transactionID)
                             .Include(ns => ns.Node)
                             .ToListAsync();

            foreach (var transactionNode in transactionNodes)
            {
                try
                {
                    if(transactionNode.TransactionState == Enums.TransactionStateType.Done)
                    {
                        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
                       _ = await (transactionNode.Node.Name switch
                        {
                            "ORDER.API" => _orderHttpClient.GetAsync("rollback", cts.Token),
                            "STOCK.API" => _stockHttpClient.GetAsync("rollback", cts.Token),
                            "PAYMENT.API" => _paymentHttpClient.GetAsync("rollback", cts.Token),
                            _ => throw new Exception("Unknown service")
                        });
                            
                    }
                    
                    transactionNode.TransactionState = Enums.TransactionStateType.Aborted;


                }
                catch(Exception ex)
                {
                    transactionNode.TransactionState = Enums.TransactionStateType.Aborted;
                    throw new Exception($"Error while rolling back service {transactionNode.Node.Name}", ex);
                }

            }
               await _context.SaveChangesAsync();


        }
    }
}
