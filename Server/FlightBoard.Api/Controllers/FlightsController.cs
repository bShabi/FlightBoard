using System.Collections.Generic;
using System.Diagnostics;
using FlightBoard.Api.hubs;
using FlightBoard.Models.Enum;
using FlightBoard.Models.Exceptions;
using FlightBoard.Models.Interface;
using FlightBoard.Models.Models;
using FlightBoard.Models.Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Net.Http.Headers;

namespace FlightBoard.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FlightsController : Controller
    {

        private readonly IFlightManager _flightManager;
        private readonly IHubContext<FlightsHub> _hubContext;

        public FlightsController(IFlightManager flightManager, IHubContext<FlightsHub> hubContext)
        {
            _flightManager = flightManager;
            _hubContext = hubContext;
        }

        [HttpGet("GetAllFlights")]
        public async Task<List<FlightResponse>> GetAllFlights()
        {

            try
            {
                List<FlightResponse> res = await _flightManager.GetAllFlightsAsync(false);
                return await Task.FromResult<List<FlightResponse>>(res);

            }
            catch (Exception ex)
            {
                throw new GeneralException(eError.REQUEST_NOT_CURRECT, eError.REQUEST_NOT_CURRECT.ToString());

            }

        }
        [HttpGet("GetFlightById/{flightId}")]
        public async Task<FlightResponse> GetFlightById(string flightId)
        {

            try
            {
                if (string.IsNullOrEmpty(flightId))
                {
                    throw new GeneralException(eError.REQUEST_NOT_CURRECT, eError.REQUEST_NOT_CURRECT.ToString());

                }
                return await _flightManager.GetFlightByGuidAsync(flightId);

            }
            catch (Exception ex)
            {
                throw new GeneralException(eError.GENERAL_ERROR, eError.GENERAL_ERROR.ToString());

            }


        }
        [HttpPut("UpdateFlight/{flightId}")]
        public async Task<FlightResponse> UpdateFlight([FromBody] FlightResponse obj, string flightId)
        {
            try
            {
                if (string.IsNullOrEmpty(flightId))
                {
                    throw new GeneralException(eError.REQUEST_NOT_CURRECT, eError.REQUEST_NOT_CURRECT.ToString());
                }
                var updated = await _flightManager.UpdateFlightAsync(obj, flightId);
                // שידור ללקוחות שזו טיסה שעודכנה
                await _hubContext.Clients.All.SendAsync("FlightUpdated", updated);
                return await Task.FromResult<FlightResponse>(updated);

            }
            catch (Exception ex)
            {
                throw new GeneralException(eError.GENERAL_ERROR, eError.GENERAL_ERROR.ToString());

            }

        }
        [HttpDelete("DeleteFlight/{flightNumber}")]
        public async Task<bool> DeleteFlight(string flightNumber)
        {
            try
            {
                if (string.IsNullOrEmpty(flightNumber))
                {
                    throw new GeneralException(eError.REQUEST_NOT_CURRECT, eError.REQUEST_NOT_CURRECT.ToString());
                }
                bool res = await _flightManager.DeleteFlightAsync(flightNumber);
                if (!res)
                {
                    throw new GeneralException(eError.FLIGHT_NOT_FOUND, eError.FLIGHT_NOT_FOUND.ToString());
                }
                await _hubContext.Clients.All.SendAsync("FlightDeleted", flightNumber);

                return await Task.FromResult<bool>(res);
            }
            catch (Exception ex)
            {
                throw new GeneralException(eError.GENERAL_ERROR, eError.GENERAL_ERROR.ToString());

            }


        }
        [HttpPost("InsertFlight")]
        public async Task<FlightResponse> InsertFlight([FromBody] FlightModel obj)
        {
            try
            {
                if (typeof(FlightModel)
                 .GetProperties()
                 .Any(prop => string.IsNullOrEmpty(prop.GetValue(obj) as string)))
                {
                    throw new GeneralException(eError.REQUEST_NOT_CURRECT, eError.REQUEST_NOT_CURRECT.ToString());
                }
                FlightResponse res = await _flightManager.AddFlightAsync(obj);

                await _hubContext.Clients.All.SendAsync("FlightAdded", res);

                return res;
            }
            catch (Exception ex)
            {
                throw new GeneralException(eError.GENERAL_ERROR, eError.GENERAL_ERROR.ToString());

            }



        }
        [HttpGet("Search")]
        public async Task<List<FlightResponse>> SearchByQueryParams([FromQuery] string? status, [FromQuery] string? destination)

        {
            try
            {        
                if (string.IsNullOrEmpty(status) && string.IsNullOrEmpty(destination))
                    throw new GeneralException(eError.REQUEST_NOT_CURRECT, eError.REQUEST_NOT_CURRECT.ToString());
            return await Task.FromResult<List<FlightResponse>>(await _flightManager.GetFlightBySearch(status, destination));

            }catch(Exception ex)
            {
                throw new GeneralException(eError.GENERAL_ERROR, eError.GENERAL_ERROR.ToString());

            }



        }
    }
}
