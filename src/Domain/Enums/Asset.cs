namespace OfficeEntry.Domain.Enums
{
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

    public enum AccessRequestReason
    {
        CriticalWork = 948160000,
        RegularWork = 948160002,
        PickupDocument = 948160001,
        PickupOfficeEquipment = 948160003,
        Other = 948160004
    }

    public static class Locale
    {
        public const string English = "en";
        public const string French = "fr";
    }
}
