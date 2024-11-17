using Microsoft.ServiceFabric.Services.Remoting;

namespace Common.Interfaces
{
    public interface IBookstore : IService
    {
        Task<List<string>> ListAvailableBooks();

        Task<string> EnlistPurchase(long bookId, uint count);

        Task<string> GetBookPrice(long bookId);

        Task<string> GetBook(long bookId);
    }
}
