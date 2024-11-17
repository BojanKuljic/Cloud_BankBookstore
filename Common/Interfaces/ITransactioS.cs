
namespace Common.Interfaces
{
    public interface ITransactioS
    {
        //pripremi transakciju
        Task<bool> Prepare(TransactionDatas context, object value);

        //ako je sve spremno  posalji novac
        Task Commit(Microsoft.ServiceFabric.Data.ITransaction transaction);

        //ako nesto nije uspelo vrati u pocetno stanje
        Task RollBack(Microsoft.ServiceFabric.Data.ITransaction transaction);
    }
}
