namespace RoomBooking.Domain.Tests;

public class LinqDeferredExecutionTests
{
    [Fact]
    public void ShouldDemonstrateDeferredExecution()
    {
        var numbers = new List<int>
        {
            1,
            2,
            3
        };

        var evenNumbers = numbers
            .Where(number => number % 2 == 0);

        numbers.Add(4);

        var result = evenNumbers.ToList();

        Assert.Equal(
            new List<int> { 2, 4 },
            result
        );
    }

    [Fact]
    public void ShouldDemonstrateImmediateMaterialization()
    {
        var numbers = new List<int>
        {
            1,
            2,
            3
        };
    
        var evenNumbers = numbers
            .Where(number => number % 2 == 0)
            .ToList();
    
        numbers.Add(4);
    
        Assert.Equal(
            new List<int> { 2 },
            evenNumbers
        );
    }
}