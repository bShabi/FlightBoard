using FlightBoard.Models.Models;
using FlightBoard.Models.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightBoard.Models.Interface
{
    public interface IFlightManager
    {
        // Define methods for managing flights here
        Task<List<FlightResponse>> GetAllFlightsAsync(bool isFromService);
        Task<FlightResponse> GetFlightByGuidAsync(string guid);
        Task<FlightResponse> AddFlightAsync(FlightModel flight);
        Task<FlightResponse> UpdateFlightAsync(FlightModel flight,string guid);
        Task<bool> DeleteFlightAsync(string guid);
        Task<List<FlightResponse>> GetFlightBySearch(string status, string destination);
        Task<bool> UpdateStatusAsync(string guid, string newStatus);
    }
}
