using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OfficeEntry.Domain.Entities;

namespace OfficeEntry.Infrastructure.Services.Xrm.Entities
{
    internal class gc_office
    {
        public Guid gc_officeid { get; set; }
        public string gc_name { get; set; }
        public gc_floor gc_floor { get; set; }
        public Shape gc_shape { get; set; }
        public string gc_coordinates { get; set; }
        public int statecode { get; set; }

        public static Office Convert(gc_office office)
        {
            if (office is null)
                return null;

            return new Office
            {
                Id = office.gc_officeid,
                Coordinates = office.gc_coordinates,
                Name = office.gc_name,
                Shape = new OptionSet
                {
                    Key = (int)office.gc_shape,
                    Value = Enum.GetName(typeof(Shape), office.gc_shape)
                }
            };
        }
    }
}
