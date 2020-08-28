using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using System;

namespace OfficeEntry.Plugins.Entities
{
	[EntityLogicalName("gc_accessrequest")]
	public partial class AccessRequest : Entity
    {
        public const string EntityLogicalName = "gc_accessrequest";
        public const int EntityTypeCode = 1;

        public AccessRequest() :
                base(EntityLogicalName)
        {
        }

		/// <summary>
		/// Unique identifier of the account.
		/// </summary>
		[AttributeLogicalName("accessrequestid")]
		public Guid? AccessRequestId
		{
			get
			{
				return GetAttributeValue<Guid?>("gc_accessrequestid");
			}
			set
			{
				SetAttributeValue("gc_accessrequestid", value);
				if (value.HasValue)
				{
					base.Id = value.Value;
				}
				else
				{
					base.Id = System.Guid.Empty;
				}
			}
		}

		[AttributeLogicalName("gc_accessrequestid")]
		public override Guid Id
		{
			get
			{
				return base.Id;
			}
			set
			{
				AccessRequestId = value;
			}
		}

		[AttributeLogicalName("gc_guid")]
		public string Guid
		{
			get
			{
				return GetAttributeValue<string>("gc_guid");
			}
			set
			{
				SetAttributeValue("gc_guid", value);
			}
		}
	}
}
