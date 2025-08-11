using FlightBoard.Models.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace FlightBoard.Models.Exceptions
{
    public class GeneralException : BaseException
    {
        public GeneralException(eError errorCode, string message) : base(message)
        {
            ErrorCode = errorCode;
        }
    }
}
