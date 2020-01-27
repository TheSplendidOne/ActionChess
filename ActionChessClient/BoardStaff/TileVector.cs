using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActionChessClient
{
    internal struct CTileVector
    {
        public Int32 XProjection { get; set; }

        public Int32 YProjection { get; set; }

        public CTileVector(Int32 xProjection, Int32 yProjection)
        {
            XProjection = xProjection;
            YProjection = yProjection;
        }

        public CTileVector(CTile beginning, CTile end) : this(end.X - beginning.X, end.Y - beginning.Y)
        {
        }
    }
}
