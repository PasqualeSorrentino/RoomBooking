using Microsoft.EntityFrameworkCore;
using RoomBooking.Domain;
using RoomBooking.Infrastructure.Data;

namespace RoomBooking.Infrastructure.Repositories;

public class EfBookingRepository : IBookingRepository
{
    private readonly RoomBookingDbContext _dbContext;

    public EfBookingRepository(
        RoomBookingDbContext dbContext
    )
    {
        _dbContext = dbContext;
    }

    public async Task<Booking?> GetByIdAsync(long id)
    {
        return await _dbContext.Bookings
            .AsNoTracking()
            .FirstOrDefaultAsync(b => b.Id == id);
    }

    public async Task SaveAsync(Booking booking)
    {
        _dbContext.Bookings.Add(booking);

        await _dbContext.SaveChangesAsync();
    }
}