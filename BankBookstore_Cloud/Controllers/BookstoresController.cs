using Client.Models;
using Common.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using Newtonsoft.Json;

namespace Client.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BookstoresController : Controller
    {
        [HttpGet]
        [Route("ListAvailableBooks")]
        public async Task<IActionResult> ListAvailableBooks()
        {
            IValidation? validationProxy = ServiceProxy.Create<IValidation>(new Uri("fabric:/Cloud_BankBookstore/Validation"));

            List<string> result = await validationProxy.ListAvailableBooks();

            if (result is null)
            {
                return RedirectToAction("Error", "Home", new { errorMessage = "Cannot get list of books." });
            }

            var books = new List<BookModel>();

            result.ForEach(x => books.Add(JsonConvert.DeserializeObject<BookModel>(x)!));

            return View(books);
        }


        [HttpGet]
        [Route("EnlistPurchase")]
        public async Task<IActionResult> EnlistPurchase(long bookId, uint count)
        {
            return View(new EnlistPurchaseModel() { BookId = bookId, Count = count });
        }

        [HttpPost]
        [Route("EnlistPurchase")]
        public async Task<IActionResult> EnlistPurchase([FromForm] EnlistPurchaseModel model)
        {
            IValidation? validationProxy = ServiceProxy.Create<IValidation>(new Uri("fabric:/Cloud_BankBookstore/Validation"));

            string result = await validationProxy.EnlistPurchase(model.BookId, model.Count);

            if (result is null)
            {
                return RedirectToAction("Error", "Home", new { errorMessage = "Cannot purchase." });
            }

            return RedirectToAction("ListAvailableBooks", "Bookstores");
        }

        [HttpGet]
        [Route("GetItemPrice/{id}")]
        public async Task<IActionResult> GetBookPrice(long id)
        {
            var validationProxy = ServiceProxy.Create<IValidation>(new Uri("fabric:/Cloud_BankBookstore/Validation"));

            var result = await validationProxy.GetBookPrice(id);

            if (result is null)
            {
                return RedirectToAction("Error", "Home", new { errorMessage = "Cannot get book's price." });
            }

            double price = JsonConvert.DeserializeObject<double>(result);

            return View(price);
        }

        [HttpGet]
        [Route("GetBook/{id}")]
        public async Task<IActionResult> GetBook(long id)
        {
            var validationProxy = ServiceProxy.Create<IValidation>(new Uri("fabric:/Cloud_BankBookstore/Validation"));

            string result = await validationProxy.GetBook(id);

            if (result is null)
            {
                return RedirectToAction("Error", "Home", new { errorMessage = "Cannot get book." });
            }

            BookModel book = JsonConvert.DeserializeObject<BookModel>(result)!;

            return View(book);
        }




    }
}
