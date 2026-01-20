using MemoryPack;

namespace OfficeEntry.Domain.Entities
{
    [MemoryPackable]
    public partial class SystemUser
    {
        public Guid Id { get; set; }
        public string DomainName { get; set; }
    }
}
