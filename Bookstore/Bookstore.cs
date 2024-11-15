using System.Fabric;
using Common.Interfaces;
using Common.Models;
using Microsoft.ServiceFabric.Data;
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
                new Book { Id = "1", Author = "Ivo Andric", Title = "Na Drini cuprija", Price = 840, Quantity = 10 },
                new Book { Id = "2", Author = "Desanka Maksimovic", Title = "Izabrane pesme", Price = 920, Quantity = 5 },
                new Book { Id = "3", Author = "Luiza Mej Alkot", Title = "Male zene", Price = 1190, Quantity = 12 }
            };

            using var transaction = StateManager.CreateTransaction();
            foreach (var book in entities)
            {
                await bookDictionary.TryAddAsync(transaction, book.Id, book);
            }
            await transaction.CommitAsync();
        }

        public async Task<double> GetItemPrice(string bookId)
        {
            using var transaction = StateManager.CreateTransaction();
            var result = await bookDictionary.TryGetValueAsync(transaction, bookId);
            return result.HasValue ? result.Value.Price : throw new KeyNotFoundException("Book not found.");
        }

        public async Task<IEnumerable<Book>> ListAvailableItems()
        {
            var books = new List<Book>();
            using var transaction = StateManager.CreateTransaction();
            var allBooks = await bookDictionary.CreateEnumerableAsync(transaction);

            using var enumerator = allBooks.GetAsyncEnumerator();
            while (await enumerator.MoveNextAsync(CancellationToken.None))
            {
                books.Add(enumerator.Current.Value);
            }

            return books;
        }

        public async Task<string> EnlistPurchase(long bookId, uint count)
        {
            using var transaction = StateManager.CreateTransaction();
            var result = await bookDictionary.TryGetValueAsync(transaction, bookId.ToString());
            if (!result.HasValue) return "Book not found.";

            var bookToUpdate = result.Value;
            if (bookToUpdate.Quantity < count) return "Insufficient quantity.";

            //(void)(bookToUpdate.Quantity -= Convert.ToInt32(count));
            await bookDictionary.TryUpdateAsync(transaction, bookId.ToString(), bookToUpdate, result.Value);
            await transaction.CommitAsync();
            return "Purchase enlisted successfully.";
        }

        public async Task<Book> GetItem(long bookId)
        {
            using var transaction = StateManager.CreateTransaction();
            var result = await bookDictionary.TryGetValueAsync(transaction, bookId.ToString());
            return result.HasValue ? result.Value : throw new KeyNotFoundException("Book not found.");
        }

        public async Task<bool> Prepare(MyTransaction data)
        {
            using var transaction = StateManager.CreateTransaction();
            var result = await bookDictionary.TryGetValueAsync(transaction, data.BookId.ToString());
            return result.HasValue && result.Value.Quantity >= data.BookAmount;
        }

        public async Task<bool> Commit(MyTransaction data)
        {
            using var transaction = StateManager.CreateTransaction();
            var result = await bookDictionary.TryGetValueAsync(transaction, data.BookId.ToString());
            if (!result.HasValue || result.Value.Quantity < data.BookAmount) return false;

            var bookToUpdate = result.Value;
            bookToUpdate.Quantity -= data.BookAmount;
            await bookDictionary.TryUpdateAsync(transaction, data.BookId.ToString(), bookToUpdate, result.Value);
            await transaction.CommitAsync();
            return true;
        }

        public async Task<bool> Rollback(MyTransaction data)
        {
            using var transaction = StateManager.CreateTransaction();
            var result = await bookDictionary.TryGetValueAsync(transaction, data.BookId.ToString());
            if (!result.HasValue) return false;

            var bookToUpdate = result.Value;
            bookToUpdate.Quantity += data.BookAmount;
            await bookDictionary.TryUpdateAsync(transaction, data.BookId.ToString(), bookToUpdate, result.Value);
            await transaction.CommitAsync();
            return true;
        }

        Task<string> IBookstore.GetItem(long bookId)
        {
            throw new NotImplementedException();
        }
    }
}
