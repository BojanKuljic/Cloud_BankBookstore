using Client.Models;
using Common.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using Newtonsoft.Json;
using System.Reflection;

namespace Client.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BanksController : Controller
    {
        [HttpGet]
        [Route("ListBanksClients")]
        public async Task<IActionResult> ListBanksClients()
        {
            // Proxy za validaciju
            IValidation? validationProxy = ServiceProxy.Create<IValidation>(new Uri("fabric:/Cloud_BankBookstore/Validation"));

            List<string> result = await validationProxy.ListBanksClients();

            if (result is null)
            {
                return RedirectToAction("Error", "Home", new { errorMessage = "Cannot get clients." });
            }

            // Priprema podataka za prikaz
            var clients = new List<BankClientModel>();
            result.ForEach(x => clients.Add(JsonConvert.DeserializeObject<BankClientModel>(x)!));

            // Prosledi podatke View-u
            return View(clients);
        }


        [HttpGet]
        [Route("EnlistMoneyTransfer")]
        public IActionResult EnlistMoneyTransfer()
        {
            var clients = GetBankClients(); // Pretpostavimo da vraća listu klijenata
            var model = new EnlistMoneyTransferModel
            {
                AvailableSenders = clients.Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.FirstName
                }).ToList(),
                AvailableReceivers = new List<SelectListItem>() // Pražnjenje za početak
            };

            return View(model);
        }

        private List<BankClientModel> GetBankClients()
        {
            return new List<BankClientModel>
    {
        new BankClientModel { Id = 1, FirstName = "Bojan Kuljic" },
        new BankClientModel { Id = 2, FirstName = "Stefan Milutinovic" },
        new BankClientModel { Id = 3, FirstName = "Nikolina Kovac" },
        new BankClientModel { Id = 4, FirstName = "Milko Kuljic" },
        new BankClientModel { Id = 5, FirstName = "Visnja Sekulic" }
    };
        }



        [HttpPost]
        [Route("EnlistMoneyTransfer")]
        public async Task<IActionResult> EnlistMoneyTransfer(EnlistMoneyTransferModel model)
        {
            // Provera validnosti unetih podataka
            if (!ModelState.IsValid)
            {
                return View(model); // Vraća korisnika na formu sa greškama
            }

            // Kreiraj proxy za validaciju
            IValidation? validationProxy = ServiceProxy.Create<IValidation>(new Uri("fabric:/Cloud_BankBookstore/Validation"));

            // Poziv servisa za transfer
            string result = await validationProxy.EnlistMoneyTransfer(model.UserSendId, model.UserReceiveId, model.Amount);

            // Obrada rezultata transfera
            if (result == null)
            {
                TempData["ErrorMessage"] = "Transfer nije uspeo. Pokušajte ponovo.";
                return RedirectToAction("Error", "Home");
            }

            // Uspešan transfer
            TempData["SuccessMessage"] = "Transfer je uspešno izvršen!";
            return RedirectToAction("ListBanksClients", "Banks");
        }
    }
}
