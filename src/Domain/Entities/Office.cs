using System;
using OfficeEntry.Domain.Enums;

namespace OfficeEntry.Domain.Entities
{
    public class Office
    {
        public Guid Id { get; set; }
        public Guid FloorId { get; set; }
        public string Coordinates { get; set; }
        public OptionSet Shape { get; set; }
        public string Name { get; set; }

        public string GetShape()
            => GetShape((OfficeShape) Shape.Key);

        private static string GetShape(OfficeShape shape)
            => shape switch
            {
                OfficeShape.Rectangle => "rect",
                OfficeShape.Circle => "circle",
                OfficeShape.Polygon => "poly",
                _ => "rect"
            };
    }
}