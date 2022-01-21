namespace OfficeEntry.Infrastructure.Services.Xrm.Entities;

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

public enum Asset
{
    Chair = 948160000,
    Laptop = 948160001,
    Tablet = 948160002,
    Monitor = 948160003,
    DockingStation = 948160004,
    Keyboard = 948160005,
    Mouse = 948160006,
    Cables = 948160007,
    Headset = 948160008,
    Printer = 948160009,
    Other = 948160010
}

public enum StateCode
{
    Active = 0,
    Inactive = 1
}
