using Xunit;
using Plugins;
using FakeXrmEasy;
using FakeXrmEasy.Extensions;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Metadata;
using System;
using OfficeEntry.Plugins;
using FluentAssertions;
using Moq;
using System.IO;

namespace Plugins.UnitTests
{
    public class DayOfWeekTests
    {
        public class When_creating_access_request 
        {

            private static Mock<IOrganizationService> currentUserService =  null;

            public EntityMetadata MockedData()
            {
                var metadata = new EntityMetadata {LogicalName = gc_accessrequest.EntityLogicalName};

                metadata.SetAttribute(new StringAttributeMetadata { LogicalName = "gc_starttime" });
                metadata.SetAttribute(new StringAttributeMetadata { LogicalName = "gc_dayofweek" });

                return metadata;
            }


            [Fact]
            public void Day_of_the_week_field_should_contain_data_when_created()
            {
                var dateToCheck = new DateTime(2024, 01, 30);

                var context = new XrmFakedContext();
                var metadata = MockedData();
                context.InitializeMetadata(metadata);
                
                var accessRequest = new gc_accessrequest {Id = Guid.NewGuid(), gc_starttime = dateToCheck};
                context.Initialize(accessRequest);

                context.ExecutePluginWithTarget<AccessRequestPlugin>(accessRequest);

                accessRequest.gc_dayofweek.Should().Be(new OptionSetValue((int)gc_accessrequest_gc_dayofweek.Tuesday));


            }

            [Fact]
            public void Day_of_the_week_field_should_change_data_when_updated()
            {
                var dateToCheck = new DateTime(2024, 01, 30);

                var context = new XrmFakedContext();
                var metadata = MockedData();
                context.InitializeMetadata(metadata);

                var accessRequest = new gc_accessrequest { Id = Guid.NewGuid(), gc_starttime = dateToCheck };
                context.Initialize(accessRequest);

                context.ExecutePluginWithTarget<AccessRequestPlugin>(accessRequest);

                accessRequest.gc_dayofweek.Should().Be(new OptionSetValue((int)gc_accessrequest_gc_dayofweek.Tuesday));
            }
        }
    }
}