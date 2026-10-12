namespace RoomBooking.Domain;

public record Room
{

    private string _name = string.Empty;
    private int _capacity;
    public required long Id { get; init; }
    public required string Name
    {
        get => _name;

        init
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("Name cannot be empty or whitespace.");
            }
            _name = value;
        }
    }
    public required int Capacity
    {
        get => _capacity;

        init
        {
            if (value <= 0)
            {
                throw new ArgumentException("Capacity must be greater than zero.");
            }
            _capacity = value;
        }
    }

    public Room()
    {
    } 
}