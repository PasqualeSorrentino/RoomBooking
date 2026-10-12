namespace RoomBooking.Domain;

public class InMemoryBookingRepository : IBookingRepository
{
    private readonly Dictionary<long, Booking> _bookings = new();

    public Task<Booking?> GetByIdAsync(long id)
    {
        _bookings.TryGetValue(id, out var booking);
        return Task.FromResult(booking);
    }

    public Task SaveAsync(Booking booking)
    {
        _bookings[booking.Id] = booking;
        return Task.CompletedTask;
    }
}