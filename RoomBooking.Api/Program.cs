using RoomBooking.Api.Errors;

using RoomBooking.Infrastructure.Repositories;

using Microsoft.EntityFrameworkCore;
using RoomBooking.Api.DTOs;
using RoomBooking.Domain;
using RoomBooking.Infrastructure.Data;

using RoomBooking.Api.Middleware;

using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services.AddProblemDetails();

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

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

builder.Services.AddScoped<
    IBookingRepository,
    EfBookingRepository
>();


// ==========================================================
// BUILD APPLICATION
// ==========================================================

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.MapScalarApiReference();
}

app.UseMiddleware<RequestLoggingMiddleware>();

app.UseExceptionHandler();

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
})
.WithName("GetRooms")
.WithSummary("Get all rooms")
.WithDescription("Returns all rooms available in the system")
.Produces<List<Room>>(StatusCodes.Status200OK);


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

    return room is null
        ? Results.NotFound()
        : Results.Ok(room);
})
.WithName("GetRoomById")
.WithSummary("Get a room by id")
.WithDescription("Returns a specific room identified by its id.")
.Produces<Room>(StatusCodes.Status200OK)
.ProducesProblem(StatusCodes.Status404NotFound);


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
})
.WithName("CreateRoom")
.WithSummary("Create a room")
.WithDescription("Creates a new room.")
.Produces<Room>(StatusCodes.Status201Created)
.ProducesProblem(StatusCodes.Status400BadRequest);


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
})
.WithName("UpdateRoom")
.WithSummary("Update a room")
.WithDescription("Updates the name and capacity of an existing room.")
.Produces<Room>(StatusCodes.Status200OK)
.ProducesProblem(StatusCodes.Status400BadRequest)
.ProducesProblem(StatusCodes.Status404NotFound);


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
})
.WithName("DeleteRoom")
.WithSummary("Delete a room")
.WithDescription("Deletes an existing room.")
.Produces(StatusCodes.Status204NoContent)
.ProducesProblem(StatusCodes.Status404NotFound);


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
})
.WithName("GetRoomBookings")
.WithSummary("Get bookings for a room")
.WithDescription("Returns all bookings associated with a specific room.")
.Produces<List<Booking>>(StatusCodes.Status200OK)
.ProducesProblem(StatusCodes.Status404NotFound);


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
    RoomBookingDbContext dbContext,
    IBookingRepository repository,
    Microsoft.Extensions.Options.IOptions<BookingOptions> options
) =>
{
    var rooms = await dbContext.Rooms
        .AsNoTracking()
        .ToListAsync();

    var bookings = await dbContext.Bookings
        .AsNoTracking()
        .ToListAsync();

    var service = new BookingService(
        rooms,
        bookings,
        repository,
        options
    );

    var nextId = 0L;

    var startUtc = DateTime.SpecifyKind(
        request.Start,
        DateTimeKind.Utc
    );

    var endUtc = DateTime.SpecifyKind(
        request.End,
        DateTimeKind.Utc
    );

    var booking = await service.BookAsync(
        nextId,
        request.RoomId,
        request.User,
        startUtc,
        endUtc,
        request.Attendees
    );

    return Results.Created(
        $"/bookings/{booking.Id}",
        booking
    );
})
.WithName("CreateBooking")
.WithSummary("Create a booking")
.WithDescription("Creates a booking after validating room availability, capacity, opening hours and booking duration.")
.Produces<Booking>(StatusCodes.Status201Created)
.ProducesProblem(StatusCodes.Status400BadRequest)
.ProducesProblem(StatusCodes.Status404NotFound)
.ProducesProblem(StatusCodes.Status409Conflict);


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
})
.WithName("DeleteBooking")
.WithSummary("Delete a booking")
.WithDescription("Deletes an existing booking.")
.Produces(StatusCodes.Status204NoContent)
.ProducesProblem(StatusCodes.Status404NotFound);


// ==========================================================
// START APPLICATION
// ==========================================================

app.Run();