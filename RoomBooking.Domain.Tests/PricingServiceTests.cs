using RoomBooking.Domain;

namespace RoomBooking.Domain.Tests;

public class PricingServiceTests
{
    [Theory]
    [InlineData(4, 10, 20)]
    [InlineData(8, 10, 30)]
    [InlineData(15, 10, 40)]
    public void ShouldCalculateMorningHourlyRate(
        int capacity,
        int hour,
        decimal expectedRate
    )
    {
        var room = new Room
        {
            Id = 1,
            Name = "Room A",
            Capacity = capacity
        };

        var pricingService = new PricingService();

        var rate = pricingService.CalculateHourlyRate(
            room,
            new DateTime(2026, 10, 8, hour, 0, 0)
        );

        Assert.Equal(expectedRate, rate);
    }

    [Theory]
    [InlineData(4, 14, 25)]
    [InlineData(8, 14, 35)]
    [InlineData(15, 14, 45)]
    public void ShouldCalculateAfternoonHourlyRate(
        int capacity,
        int hour,
        decimal expectedRate
    )
    {
        var room = new Room
        {
            Id = 1,
            Name = "Room A",
            Capacity = capacity
        };

        var pricingService = new PricingService();

        var rate = pricingService.CalculateHourlyRate(
            room,
            new DateTime(2026, 10, 8, hour, 0, 0)
        );

        Assert.Equal(expectedRate, rate);
    }
}