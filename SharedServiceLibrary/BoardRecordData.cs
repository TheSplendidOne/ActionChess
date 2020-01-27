using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedServiceLibrary
{
    public class CBoardRecordData
    {
        public List<CSerializedRecordAtom> Data { get; set; }

        public CBoardRecordData(List<CSerializedRecordAtom> data)
        {
            Data = data;
        }
    }
}
