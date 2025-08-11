using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;
using FlightBoard.Models.Interface;
using FlightBoard.Api.hubs;
using Microsoft.AspNetCore.SignalR;
using FlightBoard.Bl;

namespace FlightBoard.Api.Service
{
    public class FlightStatusUpdaterService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IHubContext<FlightsHub> _hubContext;

        public FlightStatusUpdaterService(IServiceScopeFactory scopeFactory, IHubContext<FlightsHub> hubContext)
        {
            _scopeFactory = scopeFactory;
            _hubContext = hubContext;

        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var flightManager = scope.ServiceProvider.GetRequiredService<IFlightManager>();

                    // בצע את הלוגיקה לעדכון סטטוסים
                    var allFlights = await flightManager.GetAllFlightsAsync(true); // נגיד שיש לך מתודה כזו

                    foreach (var flight in allFlights)
                    {
                        var newStatus = FlightStatusHelper.GetStatus(flight.DepartureTime);

                        if (flight.Status != newStatus)
                        {
                            flight.Status = newStatus;

                            await _hubContext.Clients.All.SendAsync("FlightUpdated", flight);
                        }
                    }
                }
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken); // EveryMin
            }
        }
    }
}
