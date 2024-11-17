using Common.Models;
using Microsoft.ServiceFabric.Services.Remoting;

namespace Common.Interfaces
{
    public interface IBank : IService
    {
        //dodao Banks
        Task<List<string>> ListBanksClients();

        Task<string> EnlistMoneyTransfer(long userSend, long userReceive, double amount);
    }
}
