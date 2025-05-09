using OfficeEntry.Domain.ValueObjects;

namespace OfficeEntry.Domain.Common.Models;

public class AlreadyBookedResult : Result
{
    public AlreadyBookedResult() : base(false, ["result-workstation-already-booked"])
    {
    }
}
