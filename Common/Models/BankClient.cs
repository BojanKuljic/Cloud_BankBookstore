using System.Runtime.Serialization;

namespace Common.Models
{
    [DataContract]
    public class BankClient : Human
    {
        [DataMember]
        public string? BankName { get; set; }

        [DataMember]
        public string? BankAdress { get; set; }

        //dodata adresa   mesto bankMembership
        [DataMember]
        public double? BankMoneyAmount { get; set; }
       
    }
}
