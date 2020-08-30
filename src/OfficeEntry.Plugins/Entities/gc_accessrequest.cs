using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using System;
using System.ComponentModel;
using System.Runtime.Serialization;

[assembly: ProxyTypesAssembly()]
namespace OfficeEntry.Plugins.Entities
{
    [DataContract()]
	[EntityLogicalName("gc_accessrequest")]
	public partial class gc_accessrequest : Entity, INotifyPropertyChanging, INotifyPropertyChanged
	{
        public const string EntityLogicalName = "gc_accessrequest";
        public const int EntityTypeCode = 10024;
		public event PropertyChangedEventHandler PropertyChanged;
		public event PropertyChangingEventHandler PropertyChanging;

		private void OnPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		private void OnPropertyChanging(string propertyName)
		{
			PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(propertyName));
		}

		public gc_accessrequest() :
                base(EntityLogicalName)
		{
		}

		[AttributeLogicalName("gc_accessrequestid")]
		public Guid? gc_accessrequestid
		{
			get
			{
				return GetAttributeValue<Guid?>("gc_accessrequestid");
			}
			set
			{
				OnPropertyChanging("gc_accessrequestid");
				SetAttributeValue("gc_accessrequestid", value);
				if (value.HasValue)
				{
					base.Id = value.Value;
				}
				else
				{
					base.Id = Guid.Empty;
				}
				OnPropertyChanged("gc_accessrequestid");
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
				gc_accessrequestid = value;
			}
		}

		[AttributeLogicalName("gc_guid")]
		public string gc_guid
		{
			get
			{
				return GetAttributeValue<string>("gc_guid");
			}
			set
			{
				OnPropertyChanging("gc_guid");
				SetAttributeValue("gc_guid", value);
				OnPropertyChanged("gc_guid");
			}
		}
	}
}
