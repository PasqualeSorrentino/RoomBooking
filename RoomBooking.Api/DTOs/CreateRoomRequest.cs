namespace RoomBooking.Api.DTOs;

public record CreateRoomRequest(
    string Name,
    int Capacity
);