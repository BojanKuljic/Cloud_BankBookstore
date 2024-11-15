using System.Fabric;
using Common.Interfaces;
using Common.Models;
using Microsoft.ServiceFabric.Data.Collections;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;

namespace Bank
{
    internal sealed class Bank : StatefulService, IBank
    {
        private IReliableDictionary<string, BankClient> bankDictionary = null;
        public Bank(StatefulServiceContext context) : base(context) { }

        protected override IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners()
        {
            return this.CreateServiceRemotingReplicaListeners();
        }

        protected override async Task RunAsync(CancellationToken cancellationToken)
        {
            bankDictionary = await StateManager.GetOrAddAsync<IReliableDictionary<string, BankClient>>("BankDictionary");

            var entities = new List<BankClient>
            {
                new BankClient { Id = "1", FirstName = "Petar", LastName = "Petrovic", MoneyAmount = 5320 },
                new BankClient { Id = "2", FirstName = "Marko", LastName = "Markovic", MoneyAmount = 620 },
                new BankClient { Id = "3", FirstName = "Jovana", LastName = "Jovanovic", MoneyAmount = 9700 }
            };

            using var transaction = StateManager.CreateTransaction();
            foreach (var client in entities)
            {
                await bankDictionary.TryAddAsync(transaction, client.Id.ToString(), client);
            }

            await transaction.CommitAsync();
        }

        // IBank
        public Task<IEnumerable<BankClient>> ListClients()
        {
            throw new NotImplementedException();
        }

        // ITransaction
        // Prepare - proverava da li klijent ima dovoljno novca
        public Task<bool> Prepare(MyTransaction data)
        {
            throw new NotImplementedException();
        }

        // Commit - skida novac sa računa
        public Task<bool> Commit(MyTransaction data)
        {
            throw new NotImplementedException();
        }

        // Rollback - vraća novac na račun
        public Task<bool> Rollback(MyTransaction data)
        {
            throw new NotImplementedException();
        }
    }
}
