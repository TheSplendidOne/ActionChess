using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace DataBaseAccessService
{
    [DataContract, Table("Boards")]
    public class CBoard
    {
        [Key, DataMember]
        public Guid BoardId { get; set; }

        [DataMember]
        public Guid GameId { get; set; }

        [DataMember]
        public Int32 BoardNumber { get; set; }

        public CGame Game { get; set; }

        [DataMember]
        public CRecord Record { get; set; }
    }
}
