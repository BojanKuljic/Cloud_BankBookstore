
namespace Common.Interfaces
{
    public interface ITransactioS
    {
        //pripremi transakciju
        Task<bool> Prepare(TransactionDatas context, object value);

        //ako je sve spremno  posalji novac
        Task Commit(ITransactioS transaction);

        //ako nesto nije uspelo vrati u pocetno stanje
        Task RollBack(ITransactioS transaction);
       
    }
}
