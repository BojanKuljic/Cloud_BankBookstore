
using System.Fabric;
using Common;
using Common.Interfaces;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using Microsoft.ServiceFabric.Services.Remoting.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;

namespace TransactionCoordinator
{
    /// <summary>
    /// An instance of this class is created for each service instance by the Service Fabric runtime.
    /// </summary>
    internal sealed class TransactionCoordinator : StatelessService, ITransactionCoordinator
    {
        private readonly string bookstorePath = @"fabric:/Cloud_BankBookstore/Bookstore";
        private readonly string bankPath = @"fabric:/Cloud_BankBookstore/Bank";


        public TransactionCoordinator(StatelessServiceContext context)
            : base(context)
        { }


        public async Task<List<string>> ListAvailableBooks()
        {

            IBookstore? bookstoreProxy = ServiceProxy.Create<IBookstore>(new Uri(bookstorePath), new ServicePartitionKey((int)PartiotionKeys.One));

            try
            {
                return await bookstoreProxy.ListAvailableBooks();
            }
            catch (Exception)
            {
                return null!;
            }
        }

        public async Task<string> EnlistPurchase(long? bookId, uint? count)
        {
            IBookstore? bookstoreProxy = ServiceProxy.Create<IBookstore>(new Uri(bookstorePath), new ServicePartitionKey((int)PartiotionKeys.One));

            try
            {
                return await bookstoreProxy.EnlistPurchase(bookId!.Value, count!.Value);
            }
            catch (Exception)
            {
                return null!;
            }
        }

        public async Task<string> GetBook(long? bookId)
        {

            IBookstore? bookstoreProxy = ServiceProxy.Create<IBookstore>(new Uri(bookstorePath), new ServicePartitionKey((int)PartiotionKeys.One));

            try
            {
                return await bookstoreProxy.GetBook(bookId!.Value);
            }
            catch (Exception)
            {
                return null!;
            }
        }

        public async Task<string> GetBookPrice(long? bookId)
        {
            IBookstore? bookstoreProxy = ServiceProxy.Create<IBookstore>(new Uri(bookstorePath), new ServicePartitionKey((int)PartiotionKeys.One));

            try
            {
                return await bookstoreProxy.GetBookPrice(bookId!.Value);
            }
            catch (Exception)
            {
                return null!;
            }
        }

     

        //IBANK Implementation

        public async Task<List<string>> ListBanksClients()
        {
            IBank? bankProxy = ServiceProxy.Create<IBank>(new Uri(bankPath), new ServicePartitionKey((int)PartiotionKeys.Two));

            try
            {
                return await bankProxy.ListBanksClients();
            }
            catch (Exception)
            {
                return null!;
            }
        }


        public async Task<string> EnlistMoneyTransfer(long? userSend, long? userReceive, double? amount)
        {
            IBank? bankProxy = ServiceProxy.Create<IBank>(new Uri(bankPath), new ServicePartitionKey((int)PartiotionKeys.Two));

            try
            {
                return await bankProxy.EnlistMoneyTransfer(userSend!.Value, userReceive!.Value, amount!.Value);
            }
            catch (Exception)
            {
                return null!;
            }
        }

        /// <summary>
        /// Optional override to create listeners (e.g., TCP, HTTP) for this service replica to handle client or user requests.
        /// </summary>
        /// <returns>A collection of listeners.</returns>
        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
            return new ServiceInstanceListener[0];
        }

        /// <summary>
        /// This is the main entry point for your service instance.
        /// </summary>
        /// <param name="cancellationToken">Canceled when Service Fabric needs to shut down this service instance.</param>
        protected override async Task RunAsync(CancellationToken cancellationToken)
        {
            // TODO: Replace the following sample code with your own logic 
            //       or remove this RunAsync override if it's not needed in your service.

            long iterations = 0;

            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();

                ServiceEventSource.Current.ServiceMessage(this.Context, "Working-{0}", ++iterations);

                await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
            }
        }
    }
}
