using Microsoft.EntityFrameworkCore;
using RoomBooking.Api.DTOs;
using RoomBooking.Domain;
using RoomBooking.Infrastructure.Data;

using RoomBooking.Api.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<BookingOptions>(
    builder.Configuration.GetSection(BookingOptions.SectionName)
);


// ==========================================================
// DATABASE
// ==========================================================

var connectionString =
    builder.Configuration.GetConnectionString("RoomBookingDb");

builder.Services.AddDbContext<RoomBookingDbContext>(options =>
    options.UseNpgsql(connectionString)
);


// ==========================================================
// BUILD APPLICATION
// ==========================================================

var app = builder.Build();

app.UseMiddleware<RequestLoggingMiddleware>();

// ==========================================================
// ROOMS ENDPOINTS
// ==========================================================

var roomsGroup = app.MapGroup("/rooms");


// ----------------------------------------------------------
// GET /rooms
// Restituisce tutte le sale
// ----------------------------------------------------------

roomsGroup.MapGet("/", async (
    RoomBookingDbContext dbContext
) =>
{
    var rooms = await dbContext.Rooms
        .AsNoTracking()
        .ToListAsync();

    return Results.Ok(rooms);
});


// ----------------------------------------------------------
// GET /rooms/{id}
// Restituisce una singola sala
// ----------------------------------------------------------

roomsGroup.MapGet("/{id:long}", async (
    long id,
    RoomBookingDbContext dbContext
) =>
{
    var room = await dbContext.Rooms
        .AsNoTracking()
        .FirstOrDefaultAsync(r => r.Id == id);

    if (room is null)
    {
        return Results.NotFound();
    }

    return Results.Ok(room);
});


// ----------------------------------------------------------
// POST /rooms
// Crea una nuova sala
// ----------------------------------------------------------

roomsGroup.MapPost("/", async (
    CreateRoomRequest request,
    RoomBookingDbContext dbContext
) =>
{
    var room = new Room
    {
        // 0 indica a EF Core che l'ID deve essere generato dal DB
        Id = 0,
        Name = request.Name,
        Capacity = request.Capacity
    };

    dbContext.Rooms.Add(room);

    await dbContext.SaveChangesAsync();

    return Results.Created(
        $"/rooms/{room.Id}",
        room
    );
});


// ----------------------------------------------------------
// PUT /rooms/{id}
// Aggiorna una sala esistente
// ----------------------------------------------------------

roomsGroup.MapPut("/{id:long}", async (
    long id,
    UpdateRoomRequest request,
    RoomBookingDbContext dbContext
) =>
{
    var room = await dbContext.Rooms
        .FirstOrDefaultAsync(r => r.Id == id);

    if (room is null)
    {
        return Results.NotFound();
    }

    /*
     * Room è un record con proprietà init.
     * Non possiamo quindi fare:
     *
     * room.Name = request.Name;
     *
     * Creiamo invece una nuova versione tramite "with".
     */
    var updatedRoom = room with
    {
        Name = request.Name,
        Capacity = request.Capacity
    };

    /*
     * Diciamo a EF Core di copiare i nuovi valori
     * sull'entità che sta già tracciando.
     */
    dbContext.Entry(room)
        .CurrentValues
        .SetValues(updatedRoom);

    await dbContext.SaveChangesAsync();

    return Results.Ok(updatedRoom);
});


// ----------------------------------------------------------
// DELETE /rooms/{id}
// Elimina una sala
// ----------------------------------------------------------

roomsGroup.MapDelete("/{id:long}", async (
    long id,
    RoomBookingDbContext dbContext
) =>
{
    var room = await dbContext.Rooms
        .FirstOrDefaultAsync(r => r.Id == id);

    if (room is null)
    {
        return Results.NotFound();
    }

    dbContext.Rooms.Remove(room);

    await dbContext.SaveChangesAsync();

    return Results.NoContent();
});


// ----------------------------------------------------------
// GET /rooms/{id}/bookings
// Restituisce tutte le prenotazioni di una sala
// ----------------------------------------------------------

roomsGroup.MapGet("/{id:long}/bookings", async (
    long id,
    RoomBookingDbContext dbContext
) =>
{
    var roomExists = await dbContext.Rooms
        .AsNoTracking()
        .AnyAsync(r => r.Id == id);

    if (!roomExists)
    {
        return Results.NotFound();
    }

    var bookings = await dbContext.Bookings
        .AsNoTracking()
        .Where(b => b.RoomId == id)
        .OrderBy(b => b.Start)
        .ToListAsync();

    return Results.Ok(bookings);
});


// ==========================================================
// BOOKINGS ENDPOINTS
// ==========================================================

var bookingsGroup = app.MapGroup("/bookings");


// ----------------------------------------------------------
// POST /bookings
// Crea una nuova prenotazione
// ----------------------------------------------------------

bookingsGroup.MapPost("/", async (
    CreateBookingRequest request,
    RoomBookingDbContext dbContext
) =>
{
    var room = await dbContext.Rooms
        .AsNoTracking()
        .FirstOrDefaultAsync(r => r.Id == request.RoomId);

    if (room is null)
    {
        return Results.NotFound(
            $"Room {request.RoomId} not found"
        );
    }

    var booking = new Booking
    {
        // ID generato automaticamente dal database
        Id = 0,

        RoomId = request.RoomId,
        User = request.User,
        Start = request.Start,
        End = request.End,
        Attendees = request.Attendees
    };

    dbContext.Bookings.Add(booking);

    await dbContext.SaveChangesAsync();

    return Results.Created(
        $"/bookings/{booking.Id}",
        booking
    );
});


// ----------------------------------------------------------
// DELETE /bookings/{id}
// Elimina una prenotazione
// ----------------------------------------------------------

bookingsGroup.MapDelete("/{id:long}", async (
    long id,
    RoomBookingDbContext dbContext
) =>
{
    var booking = await dbContext.Bookings
        .FirstOrDefaultAsync(b => b.Id == id);

    if (booking is null)
    {
        return Results.NotFound();
    }

    dbContext.Bookings.Remove(booking);

    await dbContext.SaveChangesAsync();

    return Results.NoContent();
});


// ==========================================================
// START APPLICATION
// ==========================================================

app.Run();