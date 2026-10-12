using Microsoft.Extensions.Options;
using RoomBooking.Domain.Exceptions;

namespace RoomBooking.Domain;

public class BookingService
{
    private readonly List<Room> _rooms;
    private readonly List<Booking> _bookings;

    private readonly IBookingRepository _repository;

    private readonly BookingOptions _options;

    public BookingService(
        List<Room> rooms,
        List<Booking> bookings,
        IBookingRepository repository,
        IOptions<BookingOptions> options
        )
    {
        _rooms = rooms;
        _bookings = bookings;
        _repository = repository;
        _options = options.Value;
    }

    public async  Task<Booking> BookAsync(
        long id,
        long roomId,
        string user,
        DateTime start,
        DateTime end,
        int attendees
        )
    {
        var room = _rooms.FirstOrDefault(r => r.Id == roomId);
        if (room == null)
        {
            throw new RoomNotFoundException(roomId);
        }
        if (attendees > room.Capacity)
        {
            throw new BookingValidationException(
                "Number of attendees exceeds room capacity."
            );
        }
        if (start.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday)
        {
            throw new BookingValidationException(
                "Bookings are allowed only on weekdays."
            );
        }

        var openingTime = new TimeOnly(
            _options.OpeningHour,
            0
        );

        var closingTime = new TimeOnly(
            _options.ClosingHour,
            0
        );

        var startTime = TimeOnly.FromDateTime(start);
        var endTime = TimeOnly.FromDateTime(end);

        if (startTime < openingTime || endTime > closingTime)
        {
            throw new BookingValidationException(
                $"Bookings are allowed only between {_options.OpeningHour}:00 and {_options.ClosingHour}:00."
            );
        }

        var duration = end - start;

        if (duration > TimeSpan.FromHours(
            _options.MaxDurationHours
        ))
        {
            throw new BookingValidationException(
                $"Booking duration cannot exceed {_options.MaxDurationHours} hours."
            );
        }

        var hasOverlap = _bookings.Any(b =>
            b.RoomId == roomId &&
            ((start >= b.Start && start < b.End) ||
             (end > b.Start && end <= b.End) ||
             (start <= b.Start && end >= b.End))
        );

        if (hasOverlap)
        {
            throw new BookingConflictException(
                "Room is already booked in the selected time slot."
            );
        }

        var booking = new Booking
        {
            Id = id,
            RoomId = roomId,
            User = user,
            Start = start,
            End = end,
            Attendees = attendees
        };

        await _repository.SaveAsync(booking);

        return booking;
    }

    public List<Room> FindAvailableRooms(
        DateTime start,
        DateTime end
    )
    {
        return _rooms
            .Where(room =>
                !_bookings.Any(booking =>
                    booking.RoomId == room.Id &&
                    start < booking.End &&
                    end > booking.Start
                )
            )
            .ToList();
    }

    public double CalculateWeeklyOccupancyPercentage(
        long roomId,
        DateTime weekStart
    )
    {
        var weekEnd = weekStart.AddDays(5);

        var occupiedHours = _bookings
            .Where(booking =>
                booking.RoomId == roomId &&
                booking.Start >= weekStart &&
                booking.Start < weekEnd
            )
            .Sum(booking =>
                (booking.End - booking.Start).TotalHours
            );

        const double totalAvailableHours = 60;

        return (occupiedHours / totalAvailableHours) * 100;
    }

    public string? FindUserWithMostBookings()
    {
        return _bookings
            .GroupBy(booking => booking.User)
            .OrderByDescending(group => group.Count())
            .Select(group => group.Key)
            .FirstOrDefault();
    }

}