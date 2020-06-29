using OfficeEntry.Application.Common.Interfaces;
using System;

namespace OfficeEntry.Infrastructure.Services
{
    public class DateTimeService : IDateTime
    {
        public DateTime Now => DateTime.Now;
    }
}