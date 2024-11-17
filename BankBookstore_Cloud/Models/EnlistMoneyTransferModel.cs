using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Client.Models
{
    public class EnlistMoneyTransferModel
    {
        [Required]
        public long? UserSendId { get; set; }

        [Required(ErrorMessage = "Receiver must be selected.")]
        public long? UserReceiveId { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than zero.")]
        public double Amount { get; set; }

        public List<SelectListItem> AvailableSenders { get; set; } = new();
        public List<SelectListItem> AvailableReceivers { get; set; } = new();
    }
}
