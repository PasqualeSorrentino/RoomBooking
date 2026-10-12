namespace RoomBooking.Domain;

public interface IBookingRepository
{
    Task<Booking?> GetByIdAsync(long id);
    Task SaveAsync(Booking booking);
}