using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Client.Models
{
    [DataContract]
    public class ClientModel
    {
        [DataMember]
        [Key]
        public int ClientId { get; set; }

        [DataMember]
        [Required(ErrorMessage = "First name is required")]
        [StringLength(100, ErrorMessage = "First name cannot exceed 100 characters")]
        public string FirstName { get; set; }

        [DataMember]
        [Required(ErrorMessage = "Last name is required")]
        [StringLength(100, ErrorMessage = "Last name cannot exceed 100 characters")]
        public string LastName { get; set; }

        [DataMember]
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address format")]
        public string Email { get; set; }

        [DataMember]
        [Phone(ErrorMessage = "Invalid phone number format")]
        public string? PhoneNumber { get; set; }

        [DataMember]
        [StringLength(200, ErrorMessage = "Address cannot exceed 200 characters")]
        public string? Address { get; set; }

        [DataMember]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime DateOfBirth { get; set; }

        public ClientModel(int clientId, string firstName, string lastName, string email, string? phoneNumber, string? address, DateTime dateOfBirth)
        {
            ClientId = clientId;
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            PhoneNumber = phoneNumber;
            Address = address;
            DateOfBirth = dateOfBirth;

            // Nrejr izmene
        }

    }
}
