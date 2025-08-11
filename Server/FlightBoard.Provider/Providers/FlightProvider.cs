using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Data.Sqlite;
using FlightBoard.Models.Models;
using FlightBoard.Models.Interface;
using System.IO;
using FlightBoard.Models.Response;
using FlightBoard.Bl;
namespace FlightBoard.Provider.Providers
{
    public class FlightsProvider : IFlightProvider
    {
        private readonly string _connectionString = $"Data Source={Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "database", "flightsDB")}";
        private IDbConnection CreateConnection()
        {
            return new SqliteConnection(_connectionString);
        }

        public async Task<List<FlightResponse>> GetAllFlightsAsync()
        {
            using (var connection = CreateConnection())
            {
                var sql = "SELECT * FROM flightsBoard";
                var result = await connection.QueryAsync<FlightResponse>(sql);

                return result.ToList();
            }
        }

        public async Task<FlightResponse> AddFlightAsync(FlightModel flight)
        {
            var flightResponse = new FlightResponse
            {
                Guid = Guid.NewGuid().ToString(),
                FlightNumber = flight.FlightNumber,
                Destination = flight.Destination,
                DepartureTime = flight.DepartureTime,
                Gate = flight.Gate,
                Status = FlightStatusHelper.GetStatus(flight.DepartureTime)

            };
            using (var connection = CreateConnection())
            {
                var sql = @"INSERT INTO flightsBoard (Guid, FlightNumber, Destination, DepartureTime, Gate,Status)
                    VALUES (@Guid, @FlightNumber, @Destination, @DepartureTime, @Gate, @Status)";
                int rowsAffected = await connection.ExecuteAsync(sql, flightResponse);
                if (rowsAffected > 0)
                {
                    return flightResponse; // Return the added flight
                }
                else
                {
                    throw new Exception("Failed to add flight.");
                }
            }

        }

        //public async Task<FlightResponse> UpdateFlightAsync(FlightModel flight)
        //{
        //    using (var connection = CreateConnection())
        //    {
        //        var sql = @"UPDATE flightsBoard SET 
        //                FlightNumber = @FlightNumber,
        //                Destination = @Destination,
        //                DepartureTime = @DepartureTime,
        //                Gate = @Gate,
        //                CreateDate = @CreateDate
        //            WHERE Guid = @Guid";
        //        await connection.ExecuteAsync(sql, flight);
        //    }

        //}

        public async Task<bool> DeleteFlightAsync(string flightNumber)
        {
            using (var connection = CreateConnection())
            {
                var sql = "DELETE FROM flightsBoard WHERE FlightNumber = @FlightNumber";
                int rowsAffected = await connection.ExecuteAsync(sql, new { FlightNumber = flightNumber });
                return rowsAffected > 0;
            }
        }

        public async Task<FlightResponse> GetFlightByGuidAsync(string guid)
        {
            using (var connection = CreateConnection())
            {
                var sql = "SELECT * FROM flightsBoard WHERE Guid = @Guid";
                return await connection.QueryFirstOrDefaultAsync<FlightResponse>(sql, new { Guid = guid });
            }

        }

        public async Task<bool> UpdateFlightAsync(FlightModel flight, string guid)
        {
            using (var connection = CreateConnection())
            {
                var sql = @"UPDATE flightsBoard SET 
                        FlightNumber = @FlightNumber,
                        Destination = @Destination,
                        DepartureTime = @DepartureTime,
                        Gate = @Gate
                        WHERE Guid = @Guid";

                // Use the guid parameter to ensure the correct record is updated


                int rowsAffected = await connection.ExecuteAsync(sql, flight);

                return rowsAffected > 0;
     
            }
        }
        public async Task<bool> UpdateFlightStatusAsync(string guid, string newStatus)
        {
            using (var connection = CreateConnection())
            {
                var sql = @"
                            UPDATE flightsBoard
                            SET Status = @Status
                            WHERE Guid = @Guid;";

                // Use the guid parameter to ensure the correct record is updated

                var rows = await connection.ExecuteAsync(sql, new { Guid = guid, Status = newStatus });
                return rows > 0;
            }
        }

        public async Task<bool> IsFlightExistsByFlightNumber(string flightNumber)
        {
            using (var connection = CreateConnection())
            {
                 string sql = @"SELECT COUNT(1) FROM flightsBoard WHERE FlightNumber = @FlightNumber;";
                var count = await connection.ExecuteScalarAsync<int>(sql, new { FlightNumber = flightNumber });
                return count > 0;
            }
        }
    }
}
