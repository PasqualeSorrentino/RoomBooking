namespace RoomBooking.Domain.Exceptions;

public class BookingValidationException : Exception
{
    public BookingValidationException(string message)
        : base(message)
    {
    }
}