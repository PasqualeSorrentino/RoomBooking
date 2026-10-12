using RoomBooking.Domain;

namespace RoomBooking.Domain.Tests;

public class RoomTest
{
    [Fact]
    public void ShouldCreateValidRoom()
    {
        var room = new Room
        {
            Id = 1,
            Name = "Room A",
            Capacity = 10
        };

        Assert.Equal(1, room.Id);
        Assert.Equal("Room A", room.Name);
        Assert.Equal(10, room.Capacity);
    }

    [Fact]
    public void ShouldRejectRoomWithZeroCapacity()
    {
        Assert.Throws<ArgumentException>(() =>
            new Room
            {
                Id = 1,
                Name = "Room A",
                Capacity = 0
            }
        );
    }

    [Fact]
    public void ShouldRejectRoomWithEmptyName()
    {
        Assert.Throws<ArgumentException>(() =>
            new Room
            {
                Id = 1,
                Name = "",
                Capacity = 10
            }
        );
    }

    [Fact]
    public void ShouldRejectRoomWithBlankName()
    {
        Assert.Throws<ArgumentException>(() =>
            new Room
            {
                Id = 1,
                Name = "",
                Capacity = 10
            }
        );
    }

    [Fact]
    public void ShouldRejectRoomWithNullName()
    {
        Assert.Throws<ArgumentException>(() =>
            new Room
            {
                Id = 1,
                Name = null!,
                Capacity = 10
            }
        );
    }
}