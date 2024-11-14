namespace Common
{
    public interface IBank : ITransaction
    {
        Task<IEnumerable<BankClient>> ListClients();
    }
}
