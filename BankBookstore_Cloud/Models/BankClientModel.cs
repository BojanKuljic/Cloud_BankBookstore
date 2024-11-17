using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Client.Models
{
    public class BankClientModel : HumanModel
    {    
      
        [DataMember]
        [Required(ErrorMessage = "Bank name is required")]
        [StringLength(100, ErrorMessage = "Bank name cannot exceed 20 characters")]
        public string? BankName { get; set; }


        [DataMember]
        [Required(ErrorMessage = "Address is required")]
        [StringLength(50, ErrorMessage = "Address cannot exceed 30 characters")]
        public string? BankAdress { get; set; }

        //dodata adresa   mesto bankMembership
        [DataMember]
        [Required(ErrorMessage = "Account balance is required")]
        [Range(0, Double.MaxValue, ErrorMessage = "Account balance cannot be negative")]
        public double BankMoneyAmount { get; set; }


    }
}
