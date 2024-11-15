using Common.Models;
using Microsoft.ServiceFabric.Services.Remoting;

namespace Common.Interfaces
{
    public interface ITransaction : IService
    {
        Task<bool> Prepare(MyTransaction data);
        Task<bool> Commit(MyTransaction data);
        Task<bool> Rollback(MyTransaction data);

    }
}
