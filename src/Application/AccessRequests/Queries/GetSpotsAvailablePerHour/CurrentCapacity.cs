namespace OfficeEntry.Application.AccessRequests.Queries.GetSpotsAvailablePerHour;

public class CurrentCapacity
{
    public int Hour { get; set; }
    public int Capacity { get; set; }
    public int SpotsReserved { get; set; }
}