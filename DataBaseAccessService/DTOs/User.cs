using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace DataBaseAccessService
{
    [DataContract, Table("Users")]
    public class CUser
    {
        [Key, DataMember]
        public Guid UserId { get; set; }

        [Required, DataMember]
        public String Nickname { get; set; }

        [DataMember]
        public DateTime RegistrationDate { get; set; }

        [DataMember]
        public CAuthentication Authentication { get; set; }

        public ICollection<CGame> WhiteGames { get; set; }

        public ICollection<CGame> BlackGames { get; set; }
    }
}
