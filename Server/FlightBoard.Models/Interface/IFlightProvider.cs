using FlightBoard.Models.Models;
using FlightBoard.Models.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightBoard.Models.Interface
{
    public interface IFlightProvider
    {
        Task<List<FlightResponse>> GetAllFlightsAsync();
        Task<FlightResponse> GetFlightByGuidAsync(string guid);
        Task<FlightResponse> AddFlightAsync(FlightModel flight);
        Task<bool> UpdateFlightAsync(FlightModel flight, string guid);
        Task<bool> DeleteFlightAsync(string flightNumber);
        Task<bool> IsFlightExistsByFlightNumber(string flightNumber);
        Task<bool> UpdateFlightStatusAsync(string guid, string newStatus);
    }
}
