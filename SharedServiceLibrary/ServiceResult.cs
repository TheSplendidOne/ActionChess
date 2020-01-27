using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedServiceLibrary
{
    public class CServiceResult<T>
    {
        public T Result { get; }

        public String Message { get; }

        public CServiceResult(T result, String message)
        {
            Result = result;
            Message = message;
        }

        public CServiceResult(T result) : this(result, null)
        {

        }
    }
}
