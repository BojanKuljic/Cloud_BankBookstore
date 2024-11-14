using System.Fabric;
using Common;
using Microsoft.ServiceFabric.Data.Collections;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;

namespace Bookstore
{
    internal sealed class Bookstore : StatefulService, IBookstore
    {
        private IReliableDictionary<string, Book> bookDictionary = null;
        public Bookstore(StatefulServiceContext context) : base(context) { }

        protected override IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners()
        {
            return this.CreateServiceRemotingReplicaListeners();
        }

        protected override async Task RunAsync(CancellationToken cancellationToken)
        {
            bookDictionary = await StateManager.GetOrAddAsync<IReliableDictionary<string, Book>>("BookDictionary");

            var entities = new List<Book>
            {
                new Book { Id = "knjiga1", Author = "Iva Andric", Title = "Na Drini cuprija", Price = 840, Quantity = 10 },
                new Book { Id = "knjiga2", Author = "Desanka Maksimovic", Title = "Izabrane pesme", Price = 920, Quantity = 5 },
                new Book { Id = "knjiga3", Author = "Luiza Mej Alkot", Title = "Male zene", Price = 1190, Quantity = 12 }
            };

            using var tx = StateManager.CreateTransaction();
            foreach (var book in entities)
            {
                await bookDictionary.TryAddAsync(tx, book.Id.ToString(), book);
            }

            await tx.CommitAsync();


        }

        // IBookstore
        public Task<double> GetItemPrice(string bookId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Book>> ListAvailableItems()
        {
            throw new NotImplementedException();
        }

        // ITransaction
        // Prepare - proverava da li ima dovoljno knjiga na stanju
        public Task<bool> Prepare(MyTransaction data)
        {
            throw new NotImplementedException();
        }

        // Commit - umanjuje broj knjiga za kupljeni broj
        public Task<bool> Commit(MyTransaction data)
        {
            throw new NotImplementedException();
        }

        // Rollback - vraća kupljene knjige
        public Task<bool> Rollback(MyTransaction data)
        {
            throw new NotImplementedException();
        }
    }
}
