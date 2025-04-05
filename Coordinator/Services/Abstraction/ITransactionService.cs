namespace Coordinator.Services.Abstraction
{
    public interface ITransactionService
    {
        Task<Guid> CreateTransactionAsync();
        Task PrepareServicesAsync(Guid transactionID);
        Task<bool>  CheckReadyServicesAsync(Guid transactionID);
        Task CommitAsync(Guid transactionID);
        Task<bool> CheckTransactionStateServicesAsync(Guid transactionID);

        Task RollbackAsync(Guid transactionID);





        

    }
}
