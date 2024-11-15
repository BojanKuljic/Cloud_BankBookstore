using System.Collections.Generic;
using System.Threading.Tasks;
using Common.Models;

namespace Common.Interfaces
{
    public interface IBookstore : ITransaction
    {
        Task<IEnumerable<Book>> ListAvailableItems();

        Task<double> GetItemPrice(string bookId);

        //void EnlistPurhase(string bookId, uint count);
    }
}
