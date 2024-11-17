using Common.Models;
using Microsoft.ServiceFabric.Services.Remoting;

namespace Common.Interfaces
{
    public interface ITransactionCoordinator : IService
    {
        //Kordinator transakcija koji ce vrsiti validaciju i provere
        //da li je sve ispunjeno i uredu

        // Lista dostupnih stavki vjv. knjiga

        //ZAMENIO SAM ITEMS ZA BOOKS
        Task<List<string>> ListAvailableBooks();

        // Evidentiraj kupovinu
        Task<string> EnlistPurchase(long? bookId, uint? count);

        // Preuzmi cenu stavke
        Task<string> GetBookPrice(long?  bookId);

        // Preuzmi stavku
        Task<string> GetBook(long? bookId);

        // Lista korisnika

        //DODAO SAM BANKS ISPRED CLIENTS
        Task<List<string>> ListBanksClients();

        // Evidentiraj transfer novca
        Task<string> EnlistMoneyTransfer(long?  userSend, long? userReceive, double? amount);
    }
}
