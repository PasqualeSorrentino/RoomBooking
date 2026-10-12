using Microsoft.Extensions.Options;
using RoomBooking.Domain;

namespace RoomBooking.Domain.Tests;

public class BookingServiceTests
{
    private readonly List<Room> _rooms;
    private readonly List<Booking> _bookings;
    private readonly BookingService _service;
    private readonly InMemoryBookingRepository _repository;
    private readonly IOptions<BookingOptions> _options;

    public BookingServiceTests()
    {
        _rooms = new List<Room>
        {
            new Room
            {
                Id = 1,
                Name = "Room A",
                Capacity = 10
            }
        };

        _bookings = new List<Booking>();

        _repository = new InMemoryBookingRepository();

        _options = CreateOptions();

        _service = new BookingService(
            _rooms,
            _bookings,
            _repository,
            _options
        );
    }

    private static IOptions<BookingOptions> CreateOptions()
    {
        return Options.Create(
            new BookingOptions
            {
                OpeningHour = 8,
                ClosingHour = 20,
                MaxDurationHours = 4
            }
        );
    }


    [Fact]
    public async Task ShouldRejectBookingWhenRoomDoesNotExist()
    {
        var rooms = new List<Room>();
        var bookings = new List<Booking>();

        var service = new BookingService(
            rooms,
            bookings,
            _repository,
            _options
        );

        var start = new DateTime(2026, 10, 8, 10, 0, 0);
        var end = new DateTime(2026, 10, 8, 12, 0, 0);

        await Assert.ThrowsAsync<ArgumentException>(async () =>
            await service.BookAsync(
                1,
                99,
                "John Doe",
                start,
                end,
                5
            )
        );
    }


    [Fact]
    public async Task ShouldRejectBookingWhenAttendeesExceedRoomCapacity()
    {
        var start = new DateTime(2026, 10, 8, 10, 0, 0);
        var end = new DateTime(2026, 10, 8, 12, 0, 0);

        await Assert.ThrowsAsync<ArgumentException>(async () =>
            await _service.BookAsync(
                1,
                1,
                "John Doe",
                start,
                end,
                15
            )
        );
    }


    [Theory]
    [InlineData(2026, 10, 10)] // Saturday
    [InlineData(2026, 10, 11)] // Sunday
    public async Task ShouldRejectBookingOnWeekend(
        int year,
        int month,
        int day
    )
    {
        var start = new DateTime(
            year,
            month,
            day,
            10, 0, 0
        );

        var end = new DateTime(
            year,
            month,
            day,
            12, 0, 0
        );

        await Assert.ThrowsAsync<ArgumentException>(async () =>
            await _service.BookAsync(
                1,
                1,
                "John Doe",
                start,
                end,
                5
            )
        );
    }


    [Fact]
    public async Task ShouldRejectBookingBeforeOpeningTime()
    {
        var start = new DateTime(
            2026, 10, 8,
            7, 0, 0
        );

        var end = new DateTime(
            2026, 10, 8,
            9, 0, 0
        );

        await Assert.ThrowsAsync<ArgumentException>(async () =>
            await _service.BookAsync(
                1,
                1,
                "John Doe",
                start,
                end,
                5
            )
        );
    }


    [Fact]
    public async Task ShouldRejectBookingAfterClosingTime()
    {
        var start = new DateTime(
            2026, 10, 8,
            19, 0, 0
        );

        var end = new DateTime(
            2026, 10, 8,
            20, 30, 0
        );

        await Assert.ThrowsAsync<ArgumentException>(async () =>
            await _service.BookAsync(
                1,
                1,
                "John Doe",
                start,
                end,
                5
            )
        );
    }


    [Fact]
    public async Task ShouldRejectBookingLongerThanFourHours()
    {
        var start = new DateTime(
            2026, 10, 8,
            10, 0, 0
        );

        var end = new DateTime(
            2026, 10, 8,
            15, 0, 0
        );

        await Assert.ThrowsAsync<ArgumentException>(async () =>
            await _service.BookAsync(
                1,
                1,
                "John Doe",
                start,
                end,
                5
            )
        );
    }


    [Fact]
    public async Task ShouldRejectOverlappingBookingInSameRoom()
    {
        var rooms = new List<Room>
        {
            new Room
            {
                Id = 1,
                Name = "Room A",
                Capacity = 10
            }
        };

        var bookings = new List<Booking>
        {
            new Booking
            {
                Id = 1,
                RoomId = 1,
                User = "Jane Doe",
                Start = new DateTime(
                    2026, 10, 8,
                    10, 0, 0
                ),
                End = new DateTime(
                    2026, 10, 8,
                    12, 0, 0
                ),
                Attendees = 5
            }
        };

        var service = new BookingService(
            rooms,
            bookings,
            _repository,
            _options
        );

        var start = new DateTime(
            2026, 10, 8,
            11, 0, 0
        );

        var end = new DateTime(
            2026, 10, 8,
            13, 0, 0
        );

        await Assert.ThrowsAsync<ArgumentException>(async () =>
            await service.BookAsync(
                2,
                1,
                "John Doe",
                start,
                end,
                5
            )
        );
    }


    [Fact]
    public async Task ShouldSaveBookingInRepository()
    {
        var start = new DateTime(
            2026, 10, 8,
            10, 0, 0
        );

        var end = new DateTime(
            2026, 10, 8,
            12, 0, 0
        );

        await _service.BookAsync(
            1,
            1,
            "Name",
            start,
            end,
            5
        );

        var savedBooking =
            await _repository.GetByIdAsync(1);

        Assert.NotNull(savedBooking);
        Assert.Equal(1, savedBooking.Id);
        Assert.Equal("Name", savedBooking.User);
    }


    [Fact]
    public void ShouldFindOnlyAvailableRooms()
    {
        var rooms = new List<Room>
        {
            new Room
            {
                Id = 1,
                Name = "Room A",
                Capacity = 10
            },

            new Room
            {
                Id = 2,
                Name = "Room B",
                Capacity = 20
            }
        };

        var bookings = new List<Booking>
        {
            new Booking
            {
                Id = 1,
                RoomId = 1,
                User = "Mario",
                Start = new DateTime(
                    2026, 10, 8,
                    10, 0, 0
                ),
                End = new DateTime(
                    2026, 10, 8,
                    12, 0, 0
                ),
                Attendees = 5
            }
        };

        var repository =
            new InMemoryBookingRepository();

        var service = new BookingService(
            rooms,
            bookings,
            repository,
            _options
        );

        var availableRooms =
            service.FindAvailableRooms(
                new DateTime(
                    2026, 10, 8,
                    11, 0, 0
                ),
                new DateTime(
                    2026, 10, 8,
                    13, 0, 0
                )
            );

        Assert.Single(availableRooms);
        Assert.Equal(2, availableRooms[0].Id);
    }


    [Fact]
    public void ShouldCalculateWeeklyOccupancyPercentage()
    {
        var rooms = new List<Room>
        {
            new Room
            {
                Id = 1,
                Name = "Room A",
                Capacity = 10
            }
        };

        var bookings = new List<Booking>
        {
            new Booking
            {
                Id = 1,
                RoomId = 1,
                User = "Mario",
                Start = new DateTime(
                    2026, 10, 5,
                    10, 0, 0
                ),
                End = new DateTime(
                    2026, 10, 5,
                    12, 0, 0
                ),
                Attendees = 5
            },

            new Booking
            {
                Id = 2,
                RoomId = 1,
                User = "Luigi",
                Start = new DateTime(
                    2026, 10, 6,
                    14, 0, 0
                ),
                End = new DateTime(
                    2026, 10, 6,
                    16, 0, 0
                ),
                Attendees = 4
            }
        };

        var repository =
            new InMemoryBookingRepository();

        var service = new BookingService(
            rooms,
            bookings,
            repository,
            _options
        );

        var result =
            service.CalculateWeeklyOccupancyPercentage(
                1,
                new DateTime(2026, 10, 5)
            );

        Assert.Equal(
            6.666666666666667,
            result,
            10
        );
    }


    [Fact]
    public void ShouldFindUserWithMostBookings()
    {
        var rooms = new List<Room>
        {
            new Room
            {
                Id = 1,
                Name = "Room A",
                Capacity = 10
            }
        };

        var bookings = new List<Booking>
        {
            new Booking
            {
                Id = 1,
                RoomId = 1,
                User = "Mario",
                Start = new DateTime(
                    2026, 10, 5,
                    10, 0, 0
                ),
                End = new DateTime(
                    2026, 10, 5,
                    11, 0, 0
                ),
                Attendees = 3
            },

            new Booking
            {
                Id = 2,
                RoomId = 1,
                User = "Pasquale",
                Start = new DateTime(
                    2026, 10, 6,
                    10, 0, 0
                ),
                End = new DateTime(
                    2026, 10, 6,
                    11, 0, 0
                ),
                Attendees = 3
            },

            new Booking
            {
                Id = 3,
                RoomId = 1,
                User = "Mario",
                Start = new DateTime(
                    2026, 10, 7,
                    10, 0, 0
                ),
                End = new DateTime(
                    2026, 10, 7,
                    11, 0, 0
                ),
                Attendees = 3
            }
        };

        var repository =
            new InMemoryBookingRepository();

        var service = new BookingService(
            rooms,
            bookings,
            repository,
            _options
        );

        var result =
            service.FindUserWithMostBookings();

        Assert.Equal("Mario", result);
    }


    [Fact]
    public void ShouldReturnNullWhenThereAreNoBookings()
    {
        var result =
            _service.FindUserWithMostBookings();

        Assert.Null(result);
    }


    [Fact]
    public async Task ShouldCreateValidBooking()
    {
        var start = new DateTime(
            2026, 10, 8,
            10, 0, 0
        );

        var end = new DateTime(
            2026, 10, 8,
            12, 0, 0
        );

        var booking = await _service.BookAsync(
            1,
            1,
            "John Doe",
            start,
            end,
            5
        );

        Assert.Equal(1, booking.Id);
        Assert.Equal(1, booking.RoomId);
        Assert.Equal("John Doe", booking.User);
        Assert.Equal(start, booking.Start);
        Assert.Equal(end, booking.End);
        Assert.Equal(5, booking.Attendees);
    }
}