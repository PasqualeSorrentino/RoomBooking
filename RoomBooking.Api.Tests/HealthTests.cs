namespace RoomBooking.Api.Tests;

public class HealthTests
    : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public HealthTests(
        CustomWebApplicationFactory factory
    )
    {
        factory.ResetDatabase();
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task ApplicationShouldStart()
    {
        var response = await _client.GetAsync("/rooms");

        response.EnsureSuccessStatusCode();
    }
}