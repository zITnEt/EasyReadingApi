using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyReading.Application.Exceptions
{
    public class TokensFinishedException: Exception
    {
        private const string _message = "You don't have any tokens left!";

        public TokensFinishedException()
            : base(_message) { }
 
        public TokensFinishedException(Exception innerException)
            : base(_message, innerException) { }

    }
}
