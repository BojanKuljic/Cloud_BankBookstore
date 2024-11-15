using System.Runtime.Serialization;

namespace Common.Models
{
    [DataContract]
    public class BaseEntity
    {
        [DataMember] public string? Id { get; set; }
    }
}
