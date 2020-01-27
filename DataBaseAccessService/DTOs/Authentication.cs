using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace DataBaseAccessService
{
    [DataContract, Table("Authentications")]
    public class CAuthentication
    {
        [Key, DataMember]
        public Guid UserId { get; set; }

        [Required, DataMember]
        public String Salt { get; set; }

        [Required, DataMember]
        public Byte[] PasswordHash { get; set; }

        public CUser User { get; set; }
    }
}
