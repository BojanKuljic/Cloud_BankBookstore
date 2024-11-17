using System.Runtime.Serialization;

namespace Common.Models
{
    [DataContract]
    public class Book : BaseEntity
    {

        //Izbacio sam broj strana i godinu izdavanja
        [DataMember]
        public string? Title { get; set; }

        [DataMember]
        public string? Author { get; set; }

        [DataMember]
        public double? Price { get; set; }

        [DataMember]
        public uint? Quantity { get; set; }      // Broj knjiga na stanju
    }
}
