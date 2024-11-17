using System.Runtime.Serialization;

namespace Client.Models
{
    [DataContract]
    public class BaseEntityModel
    {
        [DataMember]
        public long? Id { get; set; }

    }
}
