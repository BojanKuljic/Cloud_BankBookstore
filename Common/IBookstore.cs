using System.Collections.Generic;
using System.Threading.Tasks;

namespace Common
{
    public interface IBookstore : ITransaction
    {
        Task<IEnumerable<Book>> ListAvailableItems();

        Task<double> GetItemPrice(string bookId);

        //void EnlistPurhase(string bookId, uint count);
    }
}
