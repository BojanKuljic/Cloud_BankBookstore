using System.Runtime.Serialization;
using System.ComponentModel.DataAnnotations;

namespace Client.Models
{
    [DataContract]
    public class BookModel : BaseEntityModel
    {
         
        [DataMember]
        [Required(ErrorMessage = "Title is required")]
        [StringLength(50, ErrorMessage = "Title cannot exceed 50 characters")]
        public string? Title { get; set; }
        //public string? moze i ? na kraju kao oznaka da to polje moze biti NULL

        [DataMember]
        [Required(ErrorMessage = "Author is required")]
        [StringLength(30, ErrorMessage = "Author name cannot exceed 30 characters")]
        public string? Author { get; set; }

        [DataMember]
        [Range(0.0, 10000.0, ErrorMessage = "Price must be between 0 and 10,000 rsd")]
        public double? Price { get; set; }
        
     
        [DataMember]
        [Range(0, int.MaxValue, ErrorMessage = "Quantity cannot be negative")]
        public int? Quantity { get; set; }
               

    }
}
