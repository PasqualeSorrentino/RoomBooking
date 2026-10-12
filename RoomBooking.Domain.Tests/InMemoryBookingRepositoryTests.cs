using RoomBooking.Domain;

namespace RoomBooking.Domain.Tests;

public class InMemoryBookingRepositoryTests
{
    [Fact]
    public async Task ShouldSaveAndRetriveBooking()
    {
        var repository = new InMemoryBookingRepository();

        var booking = new Booking
        {
            Id = 1,
            RoomId = 10,
            User = "John Doe",
            Start = new DateTime(2026, 10, 8, 10, 0, 0),
            End = new DateTime(2026, 10, 8, 12, 0, 0),
            Attendees = 5
        };

        await repository.SaveAsync(booking);

        var result = await repository.GetByIdAsync(1);

        Assert.NotNull(result);
        Assert.Equal(booking, result);
    }

    [Fact]
    public async Task ShouldReturnNullWhenBookingNotExist()
    {
        var repository = new InMemoryBookingRepository();

        var result = await repository.GetByIdAsync(1);

        Assert.Null(result);
    }
}