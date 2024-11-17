using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using System.Fabric;
using Common.Interfaces;
using Microsoft.ServiceFabric.Services.Remoting.Client;

namespace Validation
{
    internal sealed class Validation : StatelessService, IValidation
    {

        private readonly string transactionCoordinatorPath = @"fabric:/Cloud_BankBookstore/TransactionCoordinator";

        public Validation(StatelessServiceContext context)
            : base(context)
        { }

        public async Task<List<string>> ListAvailableBooks()
        {
            ITransactionCoordinator? transactionProxy = ServiceProxy.Create<ITransactionCoordinator>(new Uri(transactionCoordinatorPath));

            try
            {
                return await transactionProxy.ListAvailableBooks();
            }
            catch (Exception)
            {
                return null!;
            }
        }


        public async Task<string> EnlistPurchase(long? bookId, uint? count)
        {
            if (bookId is null || count is null)
            {
                return null!;
            }

            ITransactionCoordinator? transactionProxy = ServiceProxy.Create<ITransactionCoordinator>(new Uri(transactionCoordinatorPath));

            try
            {
                return await transactionProxy.EnlistPurchase(bookId!.Value, count!.Value);
            }
            catch (Exception)
            {
                return null!;
            }
        }

        public async Task<string> GetBook(long? bookId)
        {
            if (bookId is null)
            {
                return null!;
            }

            ITransactionCoordinator? transactionProxy = ServiceProxy.Create<ITransactionCoordinator>(new Uri(transactionCoordinatorPath));

            try
            {
                return await transactionProxy.GetBook(bookId!.Value);
            }
            catch (Exception)
            {
                return null!;
            }
        }

        public async Task<string> GetBookPrice(long? bookId)
        {
            if (bookId is null)
            {
                return null!;
            }

            ITransactionCoordinator? transactionProxy = ServiceProxy.Create<ITransactionCoordinator>(new Uri(transactionCoordinatorPath));

            try
            {
                return await transactionProxy.GetBookPrice(bookId!.Value);
            }
            catch (Exception)
            {
                return null!;
            }
        }

    
        public async Task<List<string>> ListBanksClients()
        {
            ITransactionCoordinator? transactionProxy = ServiceProxy.Create<ITransactionCoordinator>(new Uri(transactionCoordinatorPath));

            try
            {
                return await transactionProxy.ListBanksClients();
            }
            catch (Exception)
            {
                return null!;
            }
        }

        public async Task<string> EnlistMoneyTransfer(long? userSend, long? userReceive, double? amount)
        {
            if (userSend is null || userReceive is null || amount is null)
            {
                return null!;
            }

            ITransactionCoordinator? transactionProxy = ServiceProxy.Create<ITransactionCoordinator>(new Uri(transactionCoordinatorPath));

            try
            {
                return await transactionProxy.EnlistMoneyTransfer(userSend!.Value, userReceive!.Value, amount!.Value);
            }
            catch (Exception)
            {
                return null!;
            }
        }

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
