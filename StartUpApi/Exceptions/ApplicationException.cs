using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StartUpApi.Exceptions
{
    /// <summary>
    /// Application Based Exception
    /// </summary>
    public class ApplicationException : Exception
    {
        /// <summary>
        /// Exception manually thrown by the application without revealing sensitive information to the frontend. 
        /// The exception message will be reported exactly to the frontend
        /// </summary>
        public ApplicationException(string message) : base(message)
        {

        }
    }
}
