namespace OfficeEntry.Application.Common.Models;

public class AlreadyBookedResult : Result
{
    internal AlreadyBookedResult() : base(false, ["result-workstation-already-booked"])
    {
    }
}
