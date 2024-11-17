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
        private IReliableDictionary<string, Book>? _bookDictionary;

        public Bookstore(StatefulServiceContext context) : base(context) { }

      

        private async Task InitializeBookDictionaryAsync()
        {
            _bookDictionary = (IReliableDictionary<string, Book>?)await StateManager.GetOrAddAsync<IReliableDictionary<long, Book>>("bookDictionary");
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
                    await _bookDictionary!.AddOrUpdateAsync(transaction, book.Id!.ToString(), book, (k, v) => v);

                //await transaction.CommitAsync();
                await FinishTransaction((ITransactioS)transaction);
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


        public async Task<string> EnlistPurchase(long bookId, uint count)
        {
            using (var transaction = StateManager.CreateTransaction())
            {
                ConditionalValue<Book> book = await _bookDictionary!.TryGetValueAsync(transaction, bookId!.ToString());
                var transactionContext = new TransactionDatas { Book = book };

                if (!await Prepare(transactionContext, count!.ToString()))
                {
                    return null!;
                }

                var bookToUpdate = book.Value;

                bookToUpdate.Quantity -= Convert.ToUInt32(count);

                await _bookDictionary.TryUpdateAsync(transaction, bookId!.ToString(), bookToUpdate, book.Value);

                //await transaction.CommitAsync();

                //return string.Empty;

                return await FinishTransaction((ITransactioS)transaction);
            }
        }

        public async Task<string> GetBookPrice(long bookId)
        {
            using (var transaction = StateManager.CreateTransaction())
            {
                var book = await _bookDictionary!.TryGetValueAsync(transaction, bookId!.ToString());

                return book.Value.Price!.ToString().ToString();
            }

            throw null!;
        }

        public async Task<string> GetBook(long bookId)
        {
            using (var transaction = StateManager.CreateTransaction())
            {
                var book = await _bookDictionary!.TryGetValueAsync(transaction, bookId!.ToString());

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


      
        public async Task Commit(ITransactioS transaction)
        {
            throw new NotImplementedException();
           // await transaction.CommitAsync();
        }

        public async Task RollBack(ITransactioS transaction)
        {
            throw new NotImplementedException();
            //transaction.Abort();
        }

        public async Task<string> FinishTransaction(ITransactioS transaction)
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



        //public async Task<double> GetBookPrice(string bookId)
        //{
        //    using var transaction = StateManager.CreateTransaction();
        //    var result = await bookDictionary.TryGetValueAsync(transaction, bookId);
        //    return result.HasValue ? result.Value.Price : throw new KeyNotFoundException("Book not found.");
        //}

        ////public async Task<IEnumerable<Book>> IBookstore.ListAvailableBooks()
        ////{
        ////    var books = new List<Book>();
        ////    using var transaction = StateManager.CreateTransaction();
        ////    var allBooks = await bookDictionary.CreateEnumerableAsync(transaction);

        ////    using var enumerator = allBooks.GetAsyncEnumerator();
        ////    while (await enumerator.MoveNextAsync(CancellationToken.None))
        ////    {
        ////        books.Add(enumerator.Current.Value);
        ////    }

        ////    return books;
        ////}

        //public async Task<string> EnlistPurchase(long bookId, uint count)
        //{
        //    using var transaction = StateManager.CreateTransaction();
        //    var result = await bookDictionary.TryGetValueAsync(transaction, bookId.ToString());
        //    if (!result.HasValue) return "Book not found.";

        //    var bookToUpdate = result.Value;
        //    if (bookToUpdate.Quantity < count) return "Insufficient quantity.";

        //    //(void)(bookToUpdate.Quantity -= Convert.ToInt32(count));
        //    await bookDictionary.TryUpdateAsync(transaction, bookId.ToString(), bookToUpdate, result.Value);
        //    await transaction.CommitAsync();
        //    return "Purchase enlisted successfully.";
        //}

        //public async Task<Book> GetBook(long bookId)
        //{
        //    using var transaction = StateManager.CreateTransaction();
        //    var result = await bookDictionary.TryGetValueAsync(transaction, bookId.ToString());
        //    return result.HasValue ? result.Value : throw new KeyNotFoundException("Book not found.");
        //}

        //public async Task<bool> Prepare(TransactionDatas context, object value)
        //{
        //    using var transaction = StateManager.CreateTransaction();
        //    var result = await bookDictionary.TryGetValueAsync(transaction, data.BookId.ToString());
        //    return result.HasValue && result.Value.Quantity >= data.BookAmount;
        //}

        //public async Task<bool> Commit(ITransactioS transaction)
        //{
        //    using var transaction = StateManager.CreateTransaction();
        //    var result = await bookDictionary.TryGetValueAsync(transaction, data.BookId.ToString());
        //    if (!result.HasValue || result.Value.Quantity < data.BookAmount) return false;

        //    var bookToUpdate = result.Value;
        //    bookToUpdate.Quantity -= data.BookAmount;
        //    await bookDictionary.TryUpdateAsync(transaction, data.BookId.ToString(), bookToUpdate, result.Value);
        //    await transaction.CommitAsync();
        //    return true;
        //}

        //public async Task<bool> Rollback(ITransactioS transaction)
        //{
        //    using var transaction = StateManager.CreateTransaction();
        //    var result = await bookDictionary.TryGetValueAsync(transaction, transaction.BookId.ToString());
        //    if (!result.HasValue) return false;

        //    var bookToUpdate = result.Value;
        //    bookToUpdate.Quantity += data.BookAmount;
        //    await bookDictionary.TryUpdateAsync(transaction, data.BookId.ToString(), bookToUpdate, result.Value);
        //    await transaction.CommitAsync();
        //    return true;
        //}

        //Task<string> IBookstore.GetBook(long bookId)
        //{
        //    throw new NotImplementedException();
        //}

        //public async Task<List<string>> ListAvailableBooks()
        //{
        //    var books = new List<Book>();
        //    using var transaction = StateManager.CreateTransaction();
        //    var allBooks = await bookDictionary.CreateEnumerableAsync(transaction);

        //    using var enumerator = allBooks.GetAsyncEnumerator();
        //    while (await enumerator.MoveNextAsync(CancellationToken.None))
        //    {
        //        books.Add(enumerator.Current.Value);
        //    }



        //    return books;
        //}

        //public async Task<string> GetBookPrice(long bookId)
        //{

        //    using var transaction = StateManager.CreateTransaction();
        //    var result = await bookDictionary.TryGetValueAsync(transaction, bookId.ToString());
        //    return result.HasValue ? result.Value.Price : throw new KeyNotFoundException("Book not found.");
        //}
    }
}
