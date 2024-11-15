using Common.Models;

namespace Common.Interfaces
{
    public interface IBank : ITransaction
    {
        Task<IEnumerable<BankClient>> ListClients();
    }
}
