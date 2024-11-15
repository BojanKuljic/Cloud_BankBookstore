using System.Collections.Generic;
using System.Threading.Tasks;
using Common.Models;

namespace Common.Interfaces
{
    public interface IBookstore : ITransaction
    {
        Task<IEnumerable<Book>> ListAvailableItems();

        Task<double> GetItemPrice(string bookId);

        Task<string> EnlistPurchase(long bookId, uint count);

        Task<string> GetItem(long bookId);
    }
}
