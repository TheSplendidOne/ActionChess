using System;
using System.Runtime.Serialization;

namespace Animator
{
    [DataContract]
    public struct CPoint
    {
        [DataMember]
        public Double X { get; set; }

        [DataMember]
        public Double Y { get; set; }

        public CPoint(Double x, Double y)
        {
            X = x;
            Y = y;
        }

        public CPoint MovePoint(CVector vector)
        {
            return new CPoint(X + vector.XProjection, Y + vector.YProjection);
        }
    }
}
