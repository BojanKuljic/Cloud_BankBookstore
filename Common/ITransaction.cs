using Microsoft.ServiceFabric.Services.Remoting;

namespace Common
{
    public interface ITransaction : IService
    {
        Task<bool> Prepare(MyTransaction data);
        Task<bool> Commit(MyTransaction data);
        Task<bool> Rollback(MyTransaction data);

    }
}
