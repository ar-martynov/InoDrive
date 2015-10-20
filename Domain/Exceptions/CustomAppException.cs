using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Exceptions
{
    [Serializable]
    public class CustomAppException : Exception
    {
        public CustomAppException(string message)
            : base(message){}
        
    }
}
