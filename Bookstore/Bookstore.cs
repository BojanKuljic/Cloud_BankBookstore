using System.Fabric;
using Common;
using Common.Interfaces;
using Common.Models;
using Microsoft.ServiceFabric.Data;
using Microsoft.ServiceFabric.Data.Collections;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using Newtonsoft.Json;

namespace Bookstore
{
    internal sealed class Bookstore : StatefulService, IBookstore, ITransactioS
    {
        private IReliableDictionary<long, Book>? _bookDictionary;

        public Bookstore(StatefulServiceContext context) : base(context) { }



        private async Task InitializeBookDictionaryAsync()
        {
            _bookDictionary = await StateManager.GetOrAddAsync<IReliableDictionary<long, Book>>("bookDictionary");
        }

        public async Task<List<string>> ListAvailableBooks()
        {
            await InitializeBookDictionaryAsync();

            var books = new List<Book>()
            {
                new Book(){ Id = 1, Title = "Bella Griva", Author = "René Guillot",   Price = 900, Quantity = 10 },
                new Book(){ Id = 2, Title = "Na Drini ćuprija", Author = "Ivo Andrić", Price = 1200, Quantity = 15 },
                new Book(){ Id = 3, Title = "Seobe", Author = "Miloš Crnjanski", Price = 1100, Quantity = 12 },
                new Book(){ Id = 4, Title = "Koreni", Author = "Dobrica Ćosić", Price = 950, Quantity = 8 },
                new Book(){ Id = 5, Title = "Zone Zamfirova", Author = "Stevan Sremac", Price = 850, Quantity = 20 },
  };

            using (var transaction = StateManager.CreateTransaction())
            {
                foreach (Book book in books)
                    await _bookDictionary!.AddOrUpdateAsync(transaction, book.Id!.Value, book, (k, v) => v);

                //await transaction.CommitAsync();
                await FinishTransaction(transaction);
            }

            var booksJson = new List<string>();

            using (var transaction = StateManager.CreateTransaction())
            {
                var enumerator = (await _bookDictionary!.CreateEnumerableAsync(transaction)).GetAsyncEnumerator();

                while (await enumerator.MoveNextAsync(CancellationToken.None))
                {
                    var book = enumerator.Current.Value;
                    booksJson.Add(JsonConvert.SerializeObject(book));
                }
            }

            return booksJson;
        }
        //IBOOKSTORE


        public async Task<string> EnlistPurchase(long? bookId, uint? count)
        {
            using (var transaction = StateManager.CreateTransaction())
            {
                ConditionalValue<Book> book = await _bookDictionary!.TryGetValueAsync(transaction, bookId!.Value);
                var transactionContext = new TransactionDatas { Book = book };

                if (!await Prepare(transactionContext, count!.Value))
                {
                    return null!;
                }

                var bookToUpdate = book.Value;

                bookToUpdate.Quantity -= Convert.ToUInt32(count);

                await _bookDictionary.TryUpdateAsync(transaction, bookId!.Value, bookToUpdate, book.Value);

                //await transaction.CommitAsync();

                //return string.Empty;

                return await FinishTransaction(transaction);
            }
        }

        public async Task<string> GetBookPrice(long? bookId)
        {
            using (var transaction = StateManager.CreateTransaction())
            {
                var book = await _bookDictionary!.TryGetValueAsync(transaction, bookId!.Value);

                return book.Value.Price!.Value.ToString();
            }

            throw null!;
        }

        public async Task<string> GetBook(long? bookId)
        {
            using (var transaction = StateManager.CreateTransaction())
            {
                var book = await _bookDictionary!.TryGetValueAsync(transaction, bookId!.Value);

                return JsonConvert.SerializeObject(book.Value);
            }

            throw null!;
        }

        // ITRANSACTIONS


        public Task<bool> Prepare(TransactionDatas context, object value)
        {
            if (!(value is uint uintParameter))
            {
                return Task.FromResult(false);
            }

            if (!context.Book.HasValue)
            {
                return Task.FromResult(false);
            }

            if (context.Book.Value.Quantity < uintParameter)
            {
                return Task.FromResult(false);
            }


            return Task.FromResult(true);
        }


      
        public async Task Commit(Microsoft.ServiceFabric.Data.ITransaction transaction)
        {
           // throw new NotImplementedException();
            await transaction.CommitAsync();
        }

        public async Task RollBack(Microsoft.ServiceFabric.Data.ITransaction transaction)
        {
           // throw new NotImplementedException();
            transaction.Abort();
        }

        public async Task<string> FinishTransaction(Microsoft.ServiceFabric.Data.ITransaction transaction)
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



        
    }
}
