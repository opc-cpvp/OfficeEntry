namespace OfficeEntry.WebApp.Models;

public class AccessRequestSubmission
{
    public Guid workspace { get; set; }
    public Guid otherIndividual { get; set; }
    public DateTime startDate { get; set; }
    public int startTime { get; set; }
    public int endTime { get; set; }
}
