using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Client.Models
{
    [DataContract]
    public class HumanModel : BaseEntityModel
    {

        [DataMember]
        [Required(ErrorMessage = "First Name is required")]
        [StringLength(50, ErrorMessage = "First Name cannot exceed 50 characters")]
        public string? FirstName { get; set; }

        [DataMember]
        [Required(ErrorMessage = "LastName is required")]
        [StringLength(50, ErrorMessage = "LastName cannot exceed 50 characters")]
        public string? LastName { get; set; }

        [DataMember]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? DateOfBirth { get; set; }

        [DataMember]
        [Required(ErrorMessage = "Gender is required")]
        [StringLength(50, ErrorMessage = "Gender cannot exceed 10 characters")]
        public string? Gender { get; set; }

    }
}
