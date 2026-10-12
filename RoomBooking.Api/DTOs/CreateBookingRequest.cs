namespace RoomBooking.Api.DTOs;

public record CreateBookingRequest(
    long RoomId,
    string User,
    DateTime Start,
    DateTime End,
    int Attendees
);