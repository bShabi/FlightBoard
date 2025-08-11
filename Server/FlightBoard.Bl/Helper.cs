using FlightBoard.Models.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightBoard.Bl
{
    public static class FlightStatusHelper
    {
        public static string GetStatus(string departureTime)
        {
            var now = DateTime.Now;
            var departureTimeParsed = DateTime.Parse(departureTime);
            var diffMinutes = (departureTimeParsed - now).TotalMinutes;

            if (diffMinutes > 30)
            {
                return eFlightStatus.Scheduled.ToString();
            }
            else if (diffMinutes <= 30 && diffMinutes >= 0)
            {
                return eFlightStatus.Boarding.ToString();
            }
            else if (diffMinutes < 0 && diffMinutes > -60)
            {
                return eFlightStatus.Departed.ToString();
            }
            else // diffMinutes < -60
            {
                return eFlightStatus.Landed.ToString();
            }
        }
    }

}
