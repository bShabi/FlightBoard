using FlightBoard.Api.hubs;
using FlightBoard.Models.Enum;
using FlightBoard.Models.Exceptions;
using FlightBoard.Models.Interface;
using FlightBoard.Models.Models;
using FlightBoard.Models.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
namespace FlightBoard.Bl.Managers
{
    public class FlightManager : IFlightManager
    {
        private readonly IFlightProvider _flightProvider;

        public FlightManager(IFlightProvider flightProvider)
        {
            _flightProvider = flightProvider;
        }
        public async Task<FlightResponse> AddFlightAsync(FlightModel flight)

        {

            if (await _flightProvider.IsFlightExistsByFlightNumber(flight.FlightNumber))
            {
                throw new GeneralException(eError.FLIGHT_NUMBER_EXISTS, "Flight number already exists.");
            }
            var flightResponse = await _flightProvider.AddFlightAsync(flight);
            return flightResponse;


        }


        public async Task<bool> DeleteFlightAsync(string flightNumber)
        {

            if (!await _flightProvider.IsFlightExistsByFlightNumber(flightNumber))
            {
                throw new GeneralException(eError.FLIGHT_NOT_FOUND, eError.FLIGHT_NOT_FOUND.ToString());

            }

            return await _flightProvider.DeleteFlightAsync(flightNumber);


        }

        public async Task<List<FlightResponse>> GetAllFlightsAsync(bool isFromService = false)
        {
            var getAllFlights = await _flightProvider.GetAllFlightsAsync();
            if (getAllFlights == null || getAllFlights.Count == 0)
                throw new GeneralException(eError.FLIGHT_NOT_FOUND, eError.FLIGHT_NOT_FOUND.ToString());
            // check if flight is status updated
            if (!isFromService)
            {
                foreach (var flight in getAllFlights)
                {
                    flight.Status = FlightStatusHelper.GetStatus(flight.DepartureTime);
                }
            }

            return getAllFlights;
        }
        public async Task<bool> UpdateStatusAsync(string guid, string newStatus)
        {
            return await _flightProvider.UpdateFlightStatusAsync(guid, newStatus);
        }

        public async Task<FlightResponse> GetFlightByGuidAsync(string guid)
        {

            var findFlight = await _flightProvider.GetFlightByGuidAsync(guid);
            if (findFlight == null)
                throw new GeneralException(eError.FLIGHT_NOT_FOUND, eError.FLIGHT_NOT_FOUND.ToString());

            return findFlight;

        }

        public async Task<FlightResponse> UpdateFlightAsync(FlightModel flight, string guid)
        {
            var res = await _flightProvider.UpdateFlightAsync(flight, guid);
            if (!res) throw new GeneralException(eError.FLIGHT_UPDATE_ERROR, eError.FLIGHT_UPDATE_ERROR.ToString());

            return new FlightResponse()
            {
                Guid = guid,
                DepartureTime = flight.DepartureTime,
                Destination = flight.Destination,
                FlightNumber = flight.FlightNumber,
                Gate = flight.Gate,
                Status = FlightStatusHelper.GetStatus(flight.DepartureTime)
            };
        }

        public async Task<List<FlightResponse>> GetFlightBySearch(string status, string destination)
        {

            var filterResponse = await _flightProvider.GetAllFlightsAsync();
            if (filterResponse == null && !filterResponse.Any())
                throw new GeneralException(eError.FLIGHT_NOT_FOUND, eError.FLIGHT_NOT_FOUND.ToString());
            if (!string.IsNullOrEmpty(status))
            {
                filterResponse = filterResponse.Where((flight) => flight.Status == status).ToList();

            }
                ;
            if (!string.IsNullOrEmpty(destination))
            {
                filterResponse = filterResponse.Where((flight) => flight.Destination.ToUpper().Contains(destination.ToUpper())).ToList();

            }

            return filterResponse;

        }

    }

}
