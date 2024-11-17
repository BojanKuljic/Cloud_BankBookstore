using Client.Models;
using Common.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using Newtonsoft.Json;

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
            IValidation? validationProxy = ServiceProxy.Create<IValidation>(new Uri("fabric:/Cloud_BankBookstore/Validation"));

            List<string> result = await validationProxy.ListBanksClients();

            if (result is null)
            {
                return RedirectToAction("Error", "Home", new { errorMessage = "Cannot get clients." });
            }

            var clients = new List<BankClientModel>();

            result.ForEach(x => clients.Add(JsonConvert.DeserializeObject<BankClientModel>(x)!));

            return View(clients);
        }


        [HttpGet]
        [Route("EnlistMoneyTransfer")]
        public async Task<IActionResult> EnlistMoneyTransfer(long userSendId)
        {
            return View(new EnlistMoneyTransferModel() { UserSendId = userSendId });
        }

        [HttpPost]
        [Route("EnlistMoneyTransfer")]
        public async Task<IActionResult> EnlistMoneyTransfer([FromForm] EnlistMoneyTransferModel model)
        {
            IValidation? validationProxy = ServiceProxy.Create<IValidation>(new Uri("fabric:/Cloud_BankBookstore/Validation"));

            string result = await validationProxy.EnlistMoneyTransfer(model.UserSendId, model.UserReceiveId, model.Amount);

            if (result is null)
            {
                return RedirectToAction("Error", "Home", new { errorMessage = "Cannot execute money transfer." });
            }

            return RedirectToAction("ListBanksClients", "Banks");
        }


    }
}
