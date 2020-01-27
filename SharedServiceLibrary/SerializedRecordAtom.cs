using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedServiceLibrary
{
    public class CSerializedRecordAtom
    {
        public EAtomType Type { get; set; }

        public String SerializedAtom { get; set; }

        public CSerializedRecordAtom(EAtomType type, String serializedAtom)
        {
            Type = type;
            SerializedAtom = serializedAtom;
        }
    }
}
