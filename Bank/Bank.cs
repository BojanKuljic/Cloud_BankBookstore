using Common;
using Common.Interfaces;
using Common.Models;
using Microsoft.ServiceFabric.Data;
using Microsoft.ServiceFabric.Data.Collections;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using Newtonsoft.Json;
using System.Fabric;
//using ITransaction = Microsoft.ServiceFabric.Data.ITransaction;

namespace Bank
{
    internal sealed class Bank : StatefulService, IBank, ITransactioS
    {
        private IReliableDictionary<string, BankClient> bankClientsDictionary;

        public Bank(StatefulServiceContext context) : base(context) { }



        private async Task InitializeClientDictionaryAsync()
        {
            bankClientsDictionary = await StateManager.GetOrAddAsync<IReliableDictionary<string, BankClient>>("bankClientsDictionary");
        }


        public async Task<List<string>> ListBanksClients()
        {
            await InitializeClientDictionaryAsync();
            var bankClients = new List<BankClient>
            {
             new BankClient() { Id = 1, FirstName = "Bojan", LastName = "Kuljic", DateOfBirth = new DateTime(2001, 12, 5), Gender= "M",
                    BankName = "NLB Bank",BankAdress="Bulevar Oslobodjenja 55", BankMoneyAmount = 1000.00 },

             new BankClient() { Id = 2, FirstName = "Stefan", LastName = "Milutinovic", DateOfBirth = new DateTime(2000, 2, 8), Gender= "M",
                    BankName = "AIK Bank",BankAdress="Bulevar Cara Lazara 12", BankMoneyAmount = 3500.00 },

             new BankClient() { Id = 3, FirstName = "Nikolina", LastName = "Kovac", DateOfBirth = new DateTime(1998, 1, 25), Gender= "Z",
                    BankName = "OTP Bank",BankAdress=" ALekse Santica 5", BankMoneyAmount = 500.00 },

             new BankClient() { Id = 4, FirstName = "Milko", LastName = "Kuljic", DateOfBirth = new DateTime(1998, 5, 31), Gender= "M",
                    BankName = "NLB Bank",BankAdress="Bulevar Oslobodjenja 55", BankMoneyAmount = 2000.00 },

             new BankClient() { Id = 5, FirstName = "Visnja", LastName = "Sekulic", DateOfBirth = new DateTime(2003, 2, 21), Gender= "Z",
                    BankName = "UniCredit Bank",BankAdress=" Cirpanoca 36", BankMoneyAmount = 900.00 },


            };

            using (var transaction = StateManager.CreateTransaction())
            {
                foreach (BankClient client in bankClients)
                    await bankClientsDictionary!.AddOrUpdateAsync(transaction, client.Id!.ToString(), client, (k, v) => v);

                //await transaction.CommitAsync();
                await FinishTransaction(transaction);
            }

            var clientsJson = new List<string>();

            using (var transaction = StateManager.CreateTransaction())
            {
                var enumerator = (await bankClientsDictionary!.CreateEnumerableAsync(transaction)).GetAsyncEnumerator();

                while (await enumerator.MoveNextAsync(CancellationToken.None))
                {
                    var client = enumerator.Current.Value;
                    clientsJson.Add(JsonConvert.SerializeObject(client));
                }
            }

            return clientsJson;
        }



        public async Task<string> EnlistMoneyTransfer(long userSend, long userReceive, double amount)
        {

            using (var transaction = StateManager.CreateTransaction())
            {
                //Value sam pretvorio u tostring
                ConditionalValue<BankClient> clientToSend = await bankClientsDictionary!.TryGetValueAsync(transaction, userSend!.ToString());
                ConditionalValue<BankClient> clientToReceive = await bankClientsDictionary!.TryGetValueAsync(transaction, userReceive!.ToString());

                var transactionContext = new TransactionDatas { BankByClientToSend = clientToSend, BankClientToReceive = clientToReceive };

                if (!await Prepare(transactionContext, amount!.ToString()))
                {
                    return null!;
                }

                var clientToSendUpdate = clientToSend.Value;
                var clientToReceiveUpdate = clientToReceive.Value;

                clientToSendUpdate.BankMoneyAmount -= amount;
                clientToReceiveUpdate.BankMoneyAmount += amount;

                await bankClientsDictionary.TryUpdateAsync(transaction, userSend!.ToString(), clientToSendUpdate, clientToSend.Value);
                await bankClientsDictionary.TryUpdateAsync(transaction, userReceive!.ToString(), clientToReceiveUpdate, clientToReceive.Value);

                //await transaction.CommitAsync();

                //return string.Empty;

                return await FinishTransaction(transaction);
            }
        }

        public async Task<bool> Prepare(TransactionDatas context, object amount)
        {
            if (!(amount is double doubleParameter))
            {
                return false;
            }

            if (!context.BankByClientToSend.HasValue || !context.BankClientToReceive.HasValue)
            {
                return false;
            }

            if (context.BankByClientToSend.Value.BankMoneyAmount < doubleParameter)
            {
                return false;
            }

            return true;
        }






        public async Task<string> FinishTransaction(ITransaction transaction)
        {
            try
            {
                await Commit(transaction);
                return string.Empty;
            }
            catch
            {
                await RollBack(transaction);
                return null!;
            }
        }

        protected override IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners()
        {
            return this.CreateServiceRemotingReplicaListeners();
        }

        public async Task Commit(ITransaction transaction)
        {
            await transaction.CommitAsync();
        }

        public async Task RollBack(ITransaction transaction)
        {
            transaction.Abort();
        }

    }

}


  