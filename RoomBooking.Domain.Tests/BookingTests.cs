using RoomBooking.Domain;

namespace RoomBooking.Domain.Tests;

public class BookingTests
{
    [Fact]
    public void ShouldCreateValidBooking()
    {
        var start = new DateTime(2026, 10, 8, 10, 0, 0);
        var end = new DateTime(2026, 10, 8, 12, 0, 0);

        var booking = new Booking
        {
            Id = 1,
            RoomId = 10,
            User = "John Doe",
            Start = start,
            End = end,
            Attendees = 5
        };

        Assert.Equal(1, booking.Id);
        Assert.Equal(10, booking.RoomId);
        Assert.Equal("John Doe", booking.User);
        Assert.Equal(start, booking.Start);
        Assert.Equal(end, booking.End);
        Assert.Equal(5, booking.Attendees);
    }

    [Fact]
    public void ShuldRejectBookWhenStartIsNotBeforeEnd()
    {
        var start = new DateTime(2026, 10, 8, 12, 0, 0);
        var end = new DateTime(2026, 10, 8, 10, 0, 0);

        Assert.Throws<ArgumentException>(() =>
            new Booking
            {
                Id = 1,
                RoomId = 10,
                User = "John Doe",
                Start = start,
                End = end,
                Attendees = 5
            }
        );
    }
}