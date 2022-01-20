using OfficeEntry.Domain.Entities;

namespace OfficeEntry.Infrastructure.Services.Xrm.Entities
{
    internal class gc_assetrequest
    {
        public string gc_name { get; set; }
        public Guid gc_assetrequestid { get; set; }
        public gc_accessrequest gc_assetsid { get; set; }
        public Asset gc_asset { get; set; }
        public string gc_other { get; set; }

        public static AssetRequest Convert(gc_assetrequest assetRequest)
        {
            if (assetRequest is null)
                return null;

            return new AssetRequest
            {
                Id = assetRequest.gc_assetrequestid,
                Asset = new OptionSet
                {
                    Key = (int)assetRequest.gc_asset,
                    Value = Enum.GetName(typeof(Asset), assetRequest.gc_asset)
                },
                Other = assetRequest.gc_other
            };
        }
    }
}
