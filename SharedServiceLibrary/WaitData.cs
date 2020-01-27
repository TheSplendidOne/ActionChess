using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedServiceLibrary
{
    public class CWaitData
    {
        public TimeSpan WaitTimeSpan { get; set; }

        public CWaitData(TimeSpan waitTimeSpan)
        {
            WaitTimeSpan = waitTimeSpan;
        }
    }
}
