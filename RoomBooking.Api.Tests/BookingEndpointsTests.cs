using System.Net;
using System.Net.Http.Json;
using RoomBooking.Api.DTOs;
using RoomBooking.Domain;

namespace RoomBooking.Api.Tests;

public class BookingEndpointsTests
    : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public BookingEndpointsTests(
        CustomWebApplicationFactory factory
    )
    {
        factory.ResetDatabase();

        _client = factory.CreateClient();
    }

    [Fact]
    public async Task CreateBooking_WithValidData_ShouldReturn201Created()
    {
        var request = new CreateBookingRequest(
            1,
            "Mario Rossi",
            new DateTime(2026, 10, 13, 10, 0, 0),
            new DateTime(2026, 10, 13, 12, 0, 0),
            4
        );

        var response = await _client.PostAsJsonAsync(
            "/bookings",
            request
        );

        Assert.Equal(
            HttpStatusCode.Created,
            response.StatusCode
        );

        var booking = await response.Content
            .ReadFromJsonAsync<Booking>();

        Assert.NotNull(booking);
        Assert.Equal(1, booking.RoomId);
        Assert.Equal("Mario Rossi", booking.User);
        Assert.Equal(4, booking.Attendees);
        Assert.True(booking.Id > 0);
    }

    [Fact]
    public async Task CreateBooking_WhenOverlapping_ShouldReturn409Conflict()
    {
        var firstBooking = new CreateBookingRequest(
            1,
            "Mario Rossi",
            new DateTime(2026, 10, 13, 10, 0, 0),
            new DateTime(2026, 10, 13, 12, 0, 0),
            4
        );

        var firstResponse = await _client.PostAsJsonAsync(
            "/bookings",
            firstBooking
        );

        Assert.Equal(
            HttpStatusCode.Created,
            firstResponse.StatusCode
        );

        var overlappingBooking = new CreateBookingRequest(
            1,
            "Luigi Verdi",
            new DateTime(2026, 10, 13, 11, 0, 0),
            new DateTime(2026, 10, 13, 13, 0, 0),
            3
        );

        var response = await _client.PostAsJsonAsync(
            "/bookings",
            overlappingBooking
        );

        Assert.Equal(
            HttpStatusCode.Conflict,
            response.StatusCode
        );
    }

    [Fact]
    public async Task CreateBooking_WhenAttendeesExceedCapacity_ShouldReturn400BadRequest()
    {
        var request = new CreateBookingRequest(
            1,
            "Anna Bianchi",
            new DateTime(2026, 10, 14, 10, 0, 0),
            new DateTime(2026, 10, 14, 12, 0, 0),
            100
        );

        var response = await _client.PostAsJsonAsync(
            "/bookings",
            request
        );

        Assert.Equal(
            HttpStatusCode.BadRequest,
            response.StatusCode
        );
    }

    [Fact]
    public async Task CreateBooking_WhenRoomDoesNotExist_ShouldReturn404NotFound()
    {
        var request = new CreateBookingRequest(
            999,
            "Mario Rossi",
            new DateTime(2026, 10, 15, 10, 0, 0),
            new DateTime(2026, 10, 15, 12, 0, 0),
            2
        );

        var response = await _client.PostAsJsonAsync(
            "/bookings",
            request
        );

        Assert.Equal(
            HttpStatusCode.NotFound,
            response.StatusCode
        );
    }

    [Fact]
    public async Task DeleteBooking_WhenBookingExists_ShouldReturn204NoContent()
    {
        var createRequest = new CreateBookingRequest(
            1,
            "Mario Rossi",
            new DateTime(2026, 10, 16, 10, 0, 0),
            new DateTime(2026, 10, 16, 12, 0, 0),
            2
        );
    
        var createResponse = await _client.PostAsJsonAsync(
            "/bookings",
            createRequest
        );
    
        Assert.Equal(
            HttpStatusCode.Created,
            createResponse.StatusCode
        );
    
        var booking = await createResponse.Content
            .ReadFromJsonAsync<Booking>();
    
        Assert.NotNull(booking);
    
        var deleteResponse = await _client.DeleteAsync(
            $"/bookings/{booking.Id}"
        );
    
        Assert.Equal(
            HttpStatusCode.NoContent,
            deleteResponse.StatusCode
        );
    }
}