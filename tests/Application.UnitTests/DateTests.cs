using FluentAssertions;
using OfficeEntry.Domain.Entities;
using Xunit;

namespace Application.UnitTests;

public class DateTests
{
    private List<AccessRequest> _list = new List<AccessRequest>()
    {
        CreateAccessRequest("Alice",  new DateTime(2022, 01, 01)),
        CreateAccessRequest("Bob",    new DateTime(2022, 01, 02)),
        CreateAccessRequest("Carole", new DateTime(2022, 01, 03)),
        CreateAccessRequest("Derek",  new DateTime(2022, 01, 02), new DateTime(2022, 01, 05)),
    };

    [Fact]
    public void Test1()
    {
        _list.Should().HaveCount(4);
    }

    [Fact]
    public void Test2()
    {
        var date = DateOnly.FromDateTime(new DateTime(2022, 01, 01));
        var startOfDay = date.ToDateTime(TimeOnly.MinValue);
        var endOfDay = date.ToDateTime(TimeOnly.MaxValue);

        var r = _list
            .Where(x => x.StartTime <= endOfDay)
            .Where(x => x.EndTime >= startOfDay)
            .OrderBy(x => x.Employee.FirstName)
            .ToArray();

        r.Should().HaveCount(1);
        r[0].Employee.FirstName.Should().Be("Alice");
    }


    [Fact]
    public void Test3()
    {
        var date = DateOnly.FromDateTime(new DateTime(2022, 01, 02));
        var startOfDay = date.ToDateTime(TimeOnly.MinValue);
        var endOfDay = date.ToDateTime(TimeOnly.MaxValue);

        var r = _list
            .Where(x => x.StartTime <= endOfDay)
            .Where(x => x.EndTime >= startOfDay)            
            .OrderBy(x => x.Employee.FirstName)
            .ToArray();

        r.Should().HaveCount(2);
        r[0].Employee.FirstName.Should().Be("Bob");
        r[1].Employee.FirstName.Should().Be("Derek");
    }

    private static AccessRequest CreateAccessRequest(string name, DateTime start, DateTime? end = null)
    {
        var endValue = end ?? start;

        var startOfDay = DateOnly.FromDateTime(start)   .ToDateTime(TimeOnly.MinValue);
        var endOfDay   = DateOnly.FromDateTime(endValue).ToDateTime(TimeOnly.MaxValue);

        return new AccessRequest
        {
            Employee = new Contact { FirstName = name },
            StartTime = startOfDay,
            EndTime = endOfDay
        };
    }
}
