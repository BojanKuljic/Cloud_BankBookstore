using System.Runtime.Serialization;

namespace Common.Models
{
    [DataContract]
    public class BankClient
    {
        [DataMember]
        public string Id { get; set; }       // Account number

        [DataMember]
        public string FirstName { get; set; }

        [DataMember]
        public string LastName { get; set; }

        [DataMember]
        public double MoneyAmount { get; set; }
    }
}
