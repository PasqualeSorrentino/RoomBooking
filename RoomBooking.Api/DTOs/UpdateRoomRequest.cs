namespace RoomBooking.Api.DTOs;

public record UpdateRoomRequest(
    string Name,
    int Capacity
);