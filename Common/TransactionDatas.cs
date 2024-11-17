using Common.Models;
using Microsoft.ServiceFabric.Data;


namespace Common
{
    public class TransactionDatas
    {
        public ConditionalValue<Book> Book { get; set; }

        public ConditionalValue<BankClient> BankByClientToSend { get; set; }

        public ConditionalValue<BankClient> BankClientToReceive { get; set; }

    }
}
