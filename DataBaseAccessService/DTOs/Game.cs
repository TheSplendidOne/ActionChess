using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace DataBaseAccessService
{
    [DataContract, Table("Games")]
    public class CGame
    {
        [Key, DataMember]
        public Guid GameId { get; set; }

        [DataMember]
        public Guid WhitePlayerId { get; set; }

        [DataMember]
        public Guid BlackPlayerId { get; set; }

        [DataMember]
        public DateTime StartGameTime { get; set; }

        [DataMember]
        public DateTime EndGameTime { get; set; }

        [DataMember]
        public Int32 WhiteScore { get; set; }

        [DataMember]
        public Int32 BlackScore { get; set; }

        [DataMember]
        public Guid WinnerId { get; set; }

        [DataMember]
        public CUser WhitePlayer { get; set; }

        [DataMember]
        public CUser BlackPlayer { get; set; }

        [DataMember]
        public ICollection<CBoard> Boards { get; set; }
    }
}
