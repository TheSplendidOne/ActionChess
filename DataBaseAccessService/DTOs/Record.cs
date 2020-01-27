using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace DataBaseAccessService
{
    [DataContract, Table("Records")]
    public class CRecord
    {
        [Key, DataMember]
        public Guid BoardId { get; set; }

        [DataMember]
        public String Record { get; set; }

        public CBoard Board { get; set; }
    }
}
