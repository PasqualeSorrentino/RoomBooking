namespace RoomBooking.Domain.Exceptions;

public class RoomNotFoundException : Exception
{
    public RoomNotFoundException(long roomId)
        : base($"Room {roomId} was not found.")
    {
    }
}