using Microsoft.ServiceFabric.Services.Communication.Runtime;

using Microsoft.ServiceFabric.Services.Runtime;
using System.Fabric;
using Common.Interfaces;

namespace Validation
{
     internal sealed class Validation : StatelessService//, IValidation
    {
        public Validation(StatelessServiceContext context)
            : base(context)
        { }

        //public Task<string> EnlistMoneyTransfer(long userSend, long userReceive, double amount)
        //{
        //    throw new NotImplementedException();
        //}

        //public Task<string> EnlistPurchase(long bookId, uint count)
        //{
        //    throw new NotImplementedException();
        //}

        //public Task<string> GetItem(long bookId)
        //{
        //    throw new NotImplementedException();
        //}

        //public Task<string> GetItemPrice(long bookId)
        //{
        //    throw new NotImplementedException();

        //}

        //public Task<List<string>> ListAvailableItems()
        //{
        //    throw new NotImplementedException();
        //}

        //public Task<List<string>> ListClients()
        //{
        //    throw new NotImplementedException();
        //}

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
