using FlightBoard.Models.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightBoard.Models.Exceptions
{
    public class BaseException : Exception
    {
        public eError ErrorCode { get; set; }

        public BaseException(string message) : base(message)
        {
        }
    }
}
