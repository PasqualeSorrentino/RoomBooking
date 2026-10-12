namespace RoomBooking.Domain;

public class BookingOptions
{
    public const string SectionName = "Booking";

    public int OpeningHour { get; set; }

    public int ClosingHour { get; set; }

    public int MaxDurationHours { get; set; }
}