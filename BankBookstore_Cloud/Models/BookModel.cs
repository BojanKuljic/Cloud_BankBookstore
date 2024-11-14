using System.Runtime.Serialization;
using System.ComponentModel.DataAnnotations;

namespace Client.Models
{
    [DataContract]
    public class BookModel
    {
     
        [DataMember]
        [Key]
        public int Id { get; set; }

        [DataMember]
        [Required(ErrorMessage = "Title is required")]
        [StringLength(50, ErrorMessage = "Title cannot exceed 50 characters")]
        public string Title { get; set; }
        //public string? moze i ? na kraju kao oznaka da to polje moze biti NULL

        [DataMember]
        [Required(ErrorMessage = "Author is required")]
        [StringLength(30, ErrorMessage = "Author name cannot exceed 30 characters")]
        public string Author { get; set; }

        [DataMember]
        [Range(0.0, 10000.0, ErrorMessage = "Price must be between 0 and 10,000 rsd")]
        public double Price { get; set; }

        [DataMember]
        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string Description { get; set; }

        [DataMember]
        [Range(1, 2000, ErrorMessage = "Pages number must be between 1 and 2 000")]
        public int PagesNumber { get; set; }

        [DataMember]
        [Range(0, int.MaxValue, ErrorMessage = "Quantity cannot be negative")]
        public int Quantity { get; set; }


        //konstruktor i implementacija u  Bank

    }
}
