using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightBoard.Models.Enum
{
    public enum eError
    {
        SUCCESS = 0,
        GENERAL_ERROR = 99,
        REQUEST_NOT_CURRECT=98,
        FLIGHT_NOT_FOUND = 97,
        FLIGHT_UPDATE_ERROR=96,
        FLIGHT_NUMBER_EXISTS=95
    }
}
