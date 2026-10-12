using Microsoft.EntityFrameworkCore;
using RoomBooking.Domain;

namespace RoomBooking.Infrastructure.Data;

public class RoomBookingDbContext : DbContext
{
    public RoomBookingDbContext(
        DbContextOptions<RoomBookingDbContext> options
    ) : base(options)
    {
    }

    public DbSet<Room> Rooms => Set<Room>();

    public DbSet<Booking> Bookings => Set<Booking>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        ConfigureRoom(modelBuilder);

        ConfigureBooking(modelBuilder);
    }

    private static void ConfigureRoom(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Room>(entity =>
        {
            entity.HasKey(room => room.Id);

            entity.Property(room => room.Name)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(room => room.Capacity)
                .IsRequired();

            entity.HasData(
            new Room
            {
                Id = 1,
                Name = "Meeting Room",
                Capacity = 6
            },
            new Room
            {
                Id = 2,
                Name = "Conference Room",
                Capacity = 20
            },
            new Room
            {
                Id = 3,
                Name = "Focus Room",
                Capacity = 2
            });
        });
    }

    private static void ConfigureBooking(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Booking>(entity =>
        {
            entity.HasKey(booking => booking.Id);

            entity.Property(booking => booking.User)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(booking => booking.Start)
                .IsRequired();

            entity.Property(booking => booking.End)
                .IsRequired();

            entity.Property(booking => booking.Attendees)
                .IsRequired();

            entity.HasIndex(booking => new
            {
                booking.RoomId,
                booking.Start
            });
        });
    }
}