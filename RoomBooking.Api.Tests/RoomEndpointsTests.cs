using System.Net;
using System.Net.Http.Json;
using RoomBooking.Api.DTOs;
using RoomBooking.Domain;

namespace RoomBooking.Api.Tests;

public class RoomEndpointsTests
    : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public RoomEndpointsTests(
        CustomWebApplicationFactory factory
    )
    {
        factory.ResetDatabase();
        
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task CreateRoom_ShouldReturn201Created()
    {
        var request = new CreateRoomRequest(
            "Integration Test Room",
            8
        );

        var response = await _client.PostAsJsonAsync(
            "/rooms",
            request
        );

        Assert.Equal(
            HttpStatusCode.Created,
            response.StatusCode
        );

        var room = await response.Content
            .ReadFromJsonAsync<Room>();

        Assert.NotNull(room);
        Assert.Equal(
            "Integration Test Room",
            room.Name
        );
        Assert.Equal(
            8,
            room.Capacity
        );
        Assert.True(room.Id > 0);
    }
}