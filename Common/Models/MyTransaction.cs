using System.Runtime.Serialization;

namespace Common.Models
{
    [DataContract]
    public class MyTransaction
    {
        [DataMember]
        public string Id { get; set; }
        [DataMember]
        public string? FirstName { get; set; }

        [DataMember] 
        public string? LastName { get; set; }

        [DataMember] 
        public string? BankName { get; set; }

        [DataMember]
        public string? BuyerId { get; set; }

        [DataMember]
        public string BookId { get; set; }

        [DataMember]
        public string BankAccount { get; set; }     // Id kupca

        [DataMember]
        public uint BookAmount { get; set; }

        [DataMember]
        public double TotalMoneyNeeded { get; set; }    // Ukupan račun (cena knjige * količina)
    }
}
