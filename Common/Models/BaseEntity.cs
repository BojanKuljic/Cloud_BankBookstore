using System.Runtime.Serialization;

namespace Common.Models
{
    [DataContract]
    public class BaseEntity
    {
        [DataMember]
        public long? Id { get; set; }
    }
}
