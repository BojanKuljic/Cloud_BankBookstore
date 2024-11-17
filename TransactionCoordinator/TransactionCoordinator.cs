using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Common.Interfaces;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
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

        public Task<string> EnlistMoneyTransfer(long userSend, long userReceive, double amount)
        {
            throw new NotImplementedException();
        }

        public Task<string> EnlistPurchase(long bookId, uint count)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetBook(long bookId)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetBookPrice(long bookId)
        {
            throw new NotImplementedException();
        }

        public Task<List<string>> ListAvailableBooks()
        {
            throw new NotImplementedException();
        }

        public Task<List<string>> ListBanksClients()
        {
            throw new NotImplementedException();
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
