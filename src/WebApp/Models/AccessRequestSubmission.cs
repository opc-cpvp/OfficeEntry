namespace OfficeEntry.WebApp.Models;

public class AccessRequestSubmission
{
    public bool registerVisitors { get; set; }
    public int chair { get; set; }
    public int laptop { get; set; }
    public int tablet { get; set; }
    public int monitor { get; set; }
    public int dockingStation { get; set; }
    public int keyboard { get; set; }
    public int mouse { get; set; }
    public int cables { get; set; }
    public int headset { get; set; }
    public int printer { get; set; }
    public Guid building { get; set; }
    public Guid floor { get; set; }
    public Guid floorplan { get; set; }
    public Guid workspace { get; set; }
    public int reason { get; set; }
    public string details { get; set; }
    public string other { get; set; }
    public Guid otherIndividual { get; set; }
    public string purpose { get; set; }
    public Visitor[] visitors { get; set; }
    public DateTime startDate { get; set; }
    public int startTime { get; set; }
    public int endTime { get; set; }
    public Guid manager { get; set; }
    public string[] acknowledgeHealth { get; set; }
    public string[] acknowledgeWorkplace { get; set; }
}
