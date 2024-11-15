using Microsoft.ServiceFabric.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models
{
    public class TransactionContext
    {

        public ConditionalValue<Book> Book { get; set; }

        public ConditionalValue<BankClient> ClientToSend { get; set; }

        public ConditionalValue<BankClient> ClientToReceive { get; set; }
    }
}
