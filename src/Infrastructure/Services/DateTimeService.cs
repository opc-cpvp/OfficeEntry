using OfficeEntry.Application.Common.Interfaces;

namespace OfficeEntry.Infrastructure.Services
{
    public class DateTimeService : IDateTime
    {
        public DateTime Now => DateTime.Now;
    }
}