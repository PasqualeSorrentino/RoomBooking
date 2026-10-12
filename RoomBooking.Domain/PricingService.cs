namespace RoomBooking.Domain;

public class PricingService
{
    public decimal CalculateHourlyRate(
        Room room,
        DateTime dateTime
    )
    {
        return (room.Capacity, dateTime.Hour) switch
        {
            (<=5, < 12) => 20m,
            (<=10, < 12) => 30m,
            (>10, < 12) => 40m,

            (<= 5, >= 12 and < 18) => 25m,
            (<= 10, >= 12 and < 18) => 35m,
            (> 10, >= 12 and < 18) => 45m,

            _ => throw new ArgumentException("Invalid room capacity or start time.")
        };
    }
}