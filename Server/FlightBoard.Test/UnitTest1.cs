using Pose;
using FlightBoard.Bl;
using FlightBoard.Models.Enum;
using FluentAssertions;
using Xunit;

namespace FlightBoard.Testing;



public interface IClock { DateTime UtcNow { get; } }

public class FakeClock : IClock
{
    public DateTime UtcNow { get; set; }
}

public class FlightStatusCalculatorTests
{
    [Theory]
    [InlineData(61, "Scheduled")]
    [InlineData(30, "Boarding")]
    [InlineData(0, "Departed")]
    [InlineData(-1, "Departed")]
    [InlineData(-61, "Landed")]
    public void GetStatus_Should_Return_Expected(int minutesFromNow, string expected)
    {
        var baseTimeUtc = DateTime.Now;
        var departure = baseTimeUtc.AddMinutes(minutesFromNow);

        var test = FlightStatusHelper.GetStatus(departure.ToString());

        test.Should().Be(expected);
    }
}
