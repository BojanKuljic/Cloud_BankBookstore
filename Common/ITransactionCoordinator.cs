using Microsoft.ServiceFabric.Services.Remoting;

namespace Common
{
    public interface ITransactionCoordinator : IService
    {
        Task<bool> CoordinateTransaction(MyTransaction data);
    }
}
