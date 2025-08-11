using FlightBoard.Models.Enum;
using FlightBoard.Models.Models;
using System.Text.Json.Serialization;


namespace FlightBoard.Models.Response
{
    public class FlightResponse: FlightModel
    {
        public string Guid { get; set; }
        public string Status { get; set; }
    }
}
