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
        public async Task<IEnumerable<BankClient>> ListClients()
        {
            var clients = new List<BankClient>();

            using (var transaction = StateManager.CreateTransaction())
            {
                var enumerator = (await bankDictionary.CreateEnumerableAsync(transaction)).GetAsyncEnumerator();

                while (await enumerator.MoveNextAsync(CancellationToken.None))
                {
                    var tmp = enumerator.Current.Value;
                    clients.Add(tmp);
                }
            }
            return clients;
        }

        // ITransaction
        // Prepare - checks if the client has enough money
        public async Task<bool> Prepare(MyTransaction data)
        {
            bool status = false;

            BankClient bankclient = null;
            var clients = await ListClients();
            foreach (var client in clients)
            {
                if (client.Id == data.BuyerId)
                {
                    bankclient = client;
                    break;
                }
            }

            if (bankclient != null)
            {

                if (!bankclient.FirstName.Equals(data.FirstName) || !bankclient.LastName.Equals(data.LastName, StringComparison.Ordinal) || !bankclient.BankName.Equals(data.BankName))
                {
                    return false;
                }

                if (bankclient.MoneyAmount >= data.BookAmount)
                {
                    status = true;
                }
            }

            return status;
        }

        // Commit - deducts money from the account
        public async Task<bool> Commit(MyTransaction data)
        {
            bool status = false;

            using (var transaction = StateManager.CreateTransaction())
            {
                try
                {
                    var clientResult = await bankDictionary.TryGetValueAsync(transaction, data.BuyerId);
                    if (clientResult.HasValue)
                    {
                        var clientFromDictionary = clientResult.Value;
                        BankClient newClient = clientFromDictionary;
                        newClient.MoneyAmount -= data.TotalMoneyNeeded;

                        if (newClient.MoneyAmount < 0)
                        {
                            status = false;
                            return status;
                        }

                        try
                        {
                            await bankDictionary.TryUpdateAsync(transaction, newClient.Id, newClient, clientFromDictionary);
                            await transaction.CommitAsync();
                            status = true;
                        }
                        catch (Exception)
                        {
                            status = false;
                            transaction.Abort();
                        }
                    }
                }
                catch (Exception)
                {
                    transaction.Abort();
                    status = false;
                }
            }

            return status;
        }

        // Rollback - returns money to the account
        public async Task<bool> Rollback(MyTransaction data)
        {
            bool status = false;

            using (var transaction = StateManager.CreateTransaction())
            {
                try
                {
                    var clientResult = await bankDictionary.TryGetValueAsync(transaction, data.BuyerId);
                    if (clientResult.HasValue)
                    {
                        var clientFromDictionary = clientResult.Value;
                        BankClient newClient = clientFromDictionary;
                        newClient.MoneyAmount += data.TotalMoneyNeeded;

                        try
                        {
                            await bankDictionary.TryUpdateAsync(transaction, newClient.Id, newClient, clientFromDictionary);
                            await transaction.CommitAsync();
                            status = true;
                        }
                        catch (Exception)
                        {
                            status = false;
                            transaction.Abort();
                        }
                    }
                }
                catch (Exception)
                {
                    transaction.Abort();
                    status = false;
                }
            }

            return status;
        }
    }
}
