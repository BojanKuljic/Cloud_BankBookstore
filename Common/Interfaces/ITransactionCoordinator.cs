using Common.Models;
using Microsoft.ServiceFabric.Services.Remoting;

namespace Common.Interfaces
{
    public interface ITransactionCoordinator : IService
    {
        Task<bool> CoordinateTransaction(MyTransaction data);
    }
}
