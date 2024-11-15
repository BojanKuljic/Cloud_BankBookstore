using Microsoft.ServiceFabric.Services.Remoting;


namespace Common.Interfaces
{
    public interface IValidation : IService
    {
        Task<string> GetItem(long bookId);

        Task<string> GetItemPrice(long bookId);

        Task<List<string>> ListClients();

        Task<List<string>> ListAvailableItems();

        Task<string> EnlistPurchase(long bookId, uint count);

        Task<string> EnlistMoneyTransfer(long userSend, long userReceive, double amount);

    }
}
