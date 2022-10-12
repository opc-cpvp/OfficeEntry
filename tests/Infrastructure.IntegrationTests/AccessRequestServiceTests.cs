using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OfficeEntry.Application.Common.Interfaces;
using OfficeEntry.Domain.Entities;
using OfficeEntry.Domain.Enums;
using Xunit;

namespace OfficeEntry.Infrastructure.IntegrationTests;

public class AccessRequestServiceTests : IDisposable
{
    private readonly IAccessRequestService _sut;

    public AccessRequestServiceTests()
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        var serviceProvider = new ServiceCollection()
            .AddInfrastructure(configuration)
            .BuildServiceProvider();

        _sut = serviceProvider.GetRequiredService<IAccessRequestService>();
    }

    [Fact]
    public async Task Create_access_resquest_no_asset_and_no_visitors()
    {
        var accessRequest = new AccessRequest
        {
            Id = Guid.NewGuid(),

            Details = "This is a fake access request (no asset, no visitors)",
            StartTime = DateTime.Now,
            EndTime = DateTime.Now.AddHours(5),
            Reason = new OptionSet { Key = 948_160_000 },

            Building = new Building { Id = Guid.Parse("{0663dbac-ddaf-ea11-a2e3-005056aa7ac3}") },
            Floor = new Floor { Id = Guid.Parse("{aada8dd4-e3af-ea11-a2e3-005056aa7ac3}") },

            Employee = new Contact { Id = Guid.Parse("{45e0ffc3-2d6c-e511-ad11-005056901d57}") },
            Manager = new Contact { Id = Guid.Parse("{edf031df-adb6-4306-bc8b-b70e77ea0d58}") }
        };

        var (result, _) = await _sut.CreateAccessRequest(accessRequest);

        Assert.True(result.Succeeded);
    }

    [Fact]
    public async Task Create_access_resquest_with_asset_and_no_visitors()
    {
        var accessRequest = new AccessRequest
        {
            Id = Guid.NewGuid(),

            Details = "This is a fake access request (with assets, no visitors)",
            StartTime = DateTime.Now,
            EndTime = DateTime.Now.AddHours(5),
            Reason = new OptionSet { Key = 948_160_000 },

            Building = new Building { Id = Guid.Parse("{0663dbac-ddaf-ea11-a2e3-005056aa7ac3}") },
            Floor = new Floor { Id = Guid.Parse("{aada8dd4-e3af-ea11-a2e3-005056aa7ac3}") },

            Employee = new Contact { Id = Guid.Parse("{45e0ffc3-2d6c-e511-ad11-005056901d57}") },
            Manager = new Contact { Id = Guid.Parse("{edf031df-adb6-4306-bc8b-b70e77ea0d58}") }
        };

        var (result, _) = await _sut.CreateAccessRequest(accessRequest);

        Assert.True(result.Succeeded);
    }

    [Fact]
    public async Task Create_access_resquest_no_asset_and_one_visitor()
    {
        var accessRequest = new AccessRequest
        {
            Id = Guid.NewGuid(),

            Details = "This is a fake access request (no asset, one visitor)",
            StartTime = DateTime.Now,
            EndTime = DateTime.Now.AddHours(5),
            Reason = new OptionSet { Key = 948_160_000 },

            Building = new Building { Id = Guid.Parse("{0663dbac-ddaf-ea11-a2e3-005056aa7ac3}") },
            Floor = new Floor { Id = Guid.Parse("{aada8dd4-e3af-ea11-a2e3-005056aa7ac3}") },

            Employee = new Contact { Id = Guid.Parse("{45e0ffc3-2d6c-e511-ad11-005056901d57}") },
            Manager = new Contact { Id = Guid.Parse("{edf031df-adb6-4306-bc8b-b70e77ea0d58}") }
        };

        var (result, _) = await _sut.CreateAccessRequest(accessRequest);

        Assert.True(result.Succeeded);
    }

    [Fact]
    public async Task Create_access_resquest_no_asset_and_two_visitors()
    {
        var accessRequest = new AccessRequest
        {
            Id = Guid.NewGuid(),

            Details = "This is a fake access request (no asset, two visitors)",
            StartTime = DateTime.Now,
            EndTime = DateTime.Now.AddHours(5),
            Reason = new OptionSet { Key = 948_160_000 },

            Building = new Building { Id = Guid.Parse("{0663dbac-ddaf-ea11-a2e3-005056aa7ac3}") },
            Floor = new Floor { Id = Guid.Parse("{aada8dd4-e3af-ea11-a2e3-005056aa7ac3}") },

            Employee = new Contact { Id = Guid.Parse("{45e0ffc3-2d6c-e511-ad11-005056901d57}") },
            Manager = new Contact { Id = Guid.Parse("{edf031df-adb6-4306-bc8b-b70e77ea0d58}") }
        };

        var (result, _) = await _sut.CreateAccessRequest(accessRequest);

        Assert.True(result.Succeeded);
    }

    [Fact]
    public async Task Create_access_resquest_with_assets_and_two_visitors()
    {
        var accessRequest = new AccessRequest
        {
            Id = Guid.NewGuid(),

            Details = "This is a fake access request (with asset, two visitors)",
            StartTime = DateTime.Now,
            EndTime = DateTime.Now.AddHours(5),
            Reason = new OptionSet { Key = 948_160_000 },

            Building = new Building { Id = Guid.Parse("{0663dbac-ddaf-ea11-a2e3-005056aa7ac3}") },
            Floor = new Floor { Id = Guid.Parse("{aada8dd4-e3af-ea11-a2e3-005056aa7ac3}") },

            Employee = new Contact { Id = Guid.Parse("{45e0ffc3-2d6c-e511-ad11-005056901d57}") },
            Manager = new Contact { Id = Guid.Parse("{edf031df-adb6-4306-bc8b-b70e77ea0d58}") }
        };

        var (result, _) = await _sut.CreateAccessRequest(accessRequest);

        Assert.True(result.Succeeded);
    }

    #region IDisposable Support
    private bool disposedValue = false; // To detect redundant calls

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                // Dispose managed state (managed objects).
                (_sut as IDisposable)?.Dispose();
            }

            disposedValue = true;
        }
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' above.
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
    #endregion
}
