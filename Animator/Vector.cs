using System;

namespace Animator
{
    public struct CVector
    {
        public Double XProjection { get; set; }

        public Double YProjection { get; set; }

        public Double Length => Math.Sqrt(Math.Pow(XProjection, 2) + Math.Pow(YProjection, 2));

        public CVector(Double xProjection, Double yProjection)
        {
            XProjection = xProjection;
            YProjection = yProjection;
        }

        public CVector(CPoint from, CPoint to)
        {
            XProjection = to.X - from.X;
            YProjection = to.Y - from.Y;
        }

        public static CVector operator *(CVector vector, Double coefficient)
        {
            return new CVector(vector.XProjection * coefficient, vector.YProjection * coefficient);
        }

        public static CVector operator *(Double coefficient, CVector vector)
        {
            return vector * coefficient;
        }
    }
}
