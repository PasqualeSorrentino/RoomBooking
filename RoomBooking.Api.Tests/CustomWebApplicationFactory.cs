using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using RoomBooking.Infrastructure.Data;

namespace RoomBooking.Api.Tests;

public class CustomWebApplicationFactory
    : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(
        IWebHostBuilder builder
    )
    {
        builder.ConfigureServices(services =>
        {
            services.RemoveAll<
                DbContextOptions<RoomBookingDbContext>
            >();

            services.RemoveAll<
                IDbContextOptionsConfiguration<RoomBookingDbContext>
            >();

            services.AddDbContext<RoomBookingDbContext>(
                options =>
                {
                    options.UseInMemoryDatabase(
                        "RoomBookingTests"
                    );
                }
            );

            var serviceProvider =
                services.BuildServiceProvider();

            using var scope =
                serviceProvider.CreateScope();

            var dbContext =
                scope.ServiceProvider
                    .GetRequiredService<RoomBookingDbContext>();

            dbContext.Database.EnsureDeleted();
            dbContext.Database.EnsureCreated();
        });
    }
    public void ResetDatabase()
    {
        using var scope = Services.CreateScope();
    
        var dbContext = scope.ServiceProvider
            .GetRequiredService<RoomBookingDbContext>();
    
        dbContext.Database.EnsureDeleted();
        dbContext.Database.EnsureCreated();
    }
}