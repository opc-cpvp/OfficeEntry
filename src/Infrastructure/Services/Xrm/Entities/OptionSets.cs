namespace OfficeEntry.Infrastructure.Services.Xrm.Entities
{

    public enum AccessReasons
    {
        CriticalWork = 948160000,
        RegularWork = 948160002,
        PickUpADocument = 948160001,
        PickUpOfficeEquipment = 948160003,
        Other = 948160004
    }

    public enum ApprovalStatus
    {
        Pending = 948160000,
        Approved = 948160001,
        Declined = 948160002,
        Cancelled = 948160003
    }
    public enum StateCode
    {
        Active = 0,
        Inactive = 1
    }
}
