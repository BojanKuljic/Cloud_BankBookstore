using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Client.Models
{
    public class BankModel
    {
        [Key]
        [DataMember]
        public int BankId { get; set; }

        [DataMember]
        [Required(ErrorMessage = "Bank name is required")]
        [StringLength(100, ErrorMessage = "Bank name cannot exceed 100 characters")]
        public string BankName { get; set; }

        [DataMember]
        [Required(ErrorMessage = "Address is required")]
        [StringLength(50, ErrorMessage = "Address cannot exceed 50 characters")]
        public string Address { get; set; }

        [DataMember]
        [Required(ErrorMessage = "Phone number is required")]
        [Phone(ErrorMessage = "Invalid phone number format")]
        public string PhoneNumber { get; set; }

        [DataMember]
        [Required(ErrorMessage = "Account balance is required")]
        [Range(0, Double.MaxValue, ErrorMessage = "Account balance cannot be negative")]
        public double AccountBalance { get; set; }


      //konstruktori i implementacija u bank service
        

    }
}
