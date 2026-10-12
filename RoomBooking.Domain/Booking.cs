namespace RoomBooking.Domain;

public record Booking
{

    private string _user = string.Empty;
    private DateTime _start;
    private DateTime _end;
    private int _attendees;
    public required long Id { get; init; }
    public required long RoomId { get; init; }
    public required string User
    {
        get => _user;
        init
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("User cannot be empty or whitespace.");
            }
            _user = value;
        }
    }
    public required DateTime Start
    {
        get => _start;
        init => _start = value;
    }
    public required DateTime End
    {
        get => _end;
        init
        {
            if (value <= Start)
            {
                throw new ArgumentException("End time must be after start time.");
            }
            _end = value;
        }
    }
    public required int Attendees
    {
        get => _attendees;
        init
        {
            if (value <= 0)
            {
                throw new ArgumentException("Attendees must be greater than zero.");
            }
            _attendees = value;
        }
    }
}