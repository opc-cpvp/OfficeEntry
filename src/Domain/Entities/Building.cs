﻿using System;

namespace OfficeEntry.Domain.Entities
{
    public class Building
    {
        public Guid Id { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Description { get; set; }
        public string Name { get; set; }
        public string Timezone { get; set; }
        public double TimezoneOffset { get; set; }

        //public IReadOnlyList<Floor> Floors { get; set; }
    }
}