using Moq;
using OfficeEntry.Application.AccessRequests.Queries.GetSpotsAvailablePerHour;
using OfficeEntry.Application.Common.Interfaces;
using OfficeEntry.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Application.UnitTests
{
    public class GetSpotsAvailablePerHourQueryHandlerTests
    {
        [Fact]
        public async Task Should_return_0_when_no_access_requests()
        {
            var moq = new Mock<IAccessRequestService>();
            moq.Setup(x => x.GetApprovedOrPendingAccessRequestsByFloor(It.IsAny<Guid>(), It.IsAny<DateTime>()))
                .ReturnsAsync(() => new AccessRequest[0]);

            var mockLocationService = new Mock<ILocationService>();
            mockLocationService.Setup(x => x.GetCapacityByFloorAsync(It.IsAny<Guid>()))
                .ReturnsAsync(20);

            var request = new GetSpotsAvailablePerHourQuery { FloorId = Guid.NewGuid(), SelectedDay = new DateTime(2020, 01, 01) };
            var handler = new GetSpotsAvailablePerHourQueryHandler(moq.Object, mockLocationService.Object);

            var result = await handler.Handle(request, CancellationToken.None);

            for (int i = 0; i < 24; i++)
            {
                Assert.Equal(0, result.First(x => x.Hour == i).SpotsReserved);
            }
        }

        [Fact]
        public async Task Should_return_1_when_1_access_requests()
        {
            var moq = new Mock<IAccessRequestService>();
            moq.Setup(x => x.GetApprovedOrPendingAccessRequestsByFloor(It.IsAny<Guid>(), It.IsAny<DateTime>()))
                .ReturnsAsync(() => new AccessRequest[]
                {
                    new AccessRequest
                    {
                        StartTime = new DateTime(2020, 01, 01).AddHours(10),
                        EndTime = new DateTime(2020, 01, 01).AddHours(10).AddMinutes(59)
                    }
                });

            var mockLocationService = new Mock<ILocationService>();
            mockLocationService.Setup(x => x.GetCapacityByFloorAsync(It.IsAny<Guid>()))
                .ReturnsAsync(20);

            var request = new GetSpotsAvailablePerHourQuery { FloorId = Guid.NewGuid(), SelectedDay = new DateTime(2020, 01, 01) };
            var handler = new GetSpotsAvailablePerHourQueryHandler(moq.Object, mockLocationService.Object);

            var result = await handler.Handle(request, CancellationToken.None);

            for (int i = 0; i < 24; i++)
            {
                if (i == 10)
                {
                    Assert.Equal(1, result.First(x => x.Hour == i).SpotsReserved);
                    continue;
                }

                Assert.Equal(0, result.First(x => x.Hour == i).SpotsReserved);
            }
        }

        [Fact]
        public async Task Should_return_1_by_hour_when_2_access_requests()
        {
            var moq = new Mock<IAccessRequestService>();
            moq.Setup(x => x.GetApprovedOrPendingAccessRequestsByFloor(It.IsAny<Guid>(), It.IsAny<DateTime>()))
                .ReturnsAsync(() => new AccessRequest[]
                {
                    new AccessRequest
                    {
                        StartTime = new DateTime(2020, 01, 01).AddHours(10),
                        EndTime = new DateTime(2020, 01, 01).AddHours(10).AddMinutes(59)
                    },
                    new AccessRequest
                    {
                        StartTime = new DateTime(2020, 01, 01).AddHours(12),
                        EndTime = new DateTime(2020, 01, 01).AddHours(12).AddMinutes(59)
                    }
                });

            var mockLocationService = new Mock<ILocationService>();
            mockLocationService.Setup(x => x.GetCapacityByFloorAsync(It.IsAny<Guid>()))
                .ReturnsAsync(20);

            var request = new GetSpotsAvailablePerHourQuery { FloorId = Guid.NewGuid(), SelectedDay = new DateTime(2020, 01, 01) };
            var handler = new GetSpotsAvailablePerHourQueryHandler(moq.Object, mockLocationService.Object);

            var result = await handler.Handle(request, CancellationToken.None);

            for (int i = 0; i < 24; i++)
            {
                if (i == 10 || i == 12)
                {
                    Assert.Equal(1, result.First(x => x.Hour == i).SpotsReserved);
                    continue;
                }

                Assert.Equal(0, result.First(x => x.Hour == i).SpotsReserved);
            }
        }

        [Fact]
        public async Task Should_return_11_by_hour_when_2_access_requests()
        {
            var moq = new Mock<IAccessRequestService>();
            moq.Setup(x => x.GetApprovedOrPendingAccessRequestsByFloor(It.IsAny<Guid>(), It.IsAny<DateTime>()))
                .ReturnsAsync(() => new AccessRequest[]
                {
                    new AccessRequest
                    {
                        StartTime = new DateTime(2020, 01, 01).AddHours(10),
                        EndTime = new DateTime(2020, 01, 01).AddHours(10).AddMinutes(59),
                        Visitors = Enumerable.Range(0, 10).Select(x => new Contact { }).ToList()
                    },
                    new AccessRequest
                    {
                        StartTime = new DateTime(2020, 01, 01).AddHours(12),
                        EndTime = new DateTime(2020, 01, 01).AddHours(12).AddMinutes(59)
                    }
                });

            var mockLocationService = new Mock<ILocationService>();
            mockLocationService.Setup(x => x.GetCapacityByFloorAsync(It.IsAny<Guid>()))
                .ReturnsAsync(20);

            var request = new GetSpotsAvailablePerHourQuery { FloorId = Guid.NewGuid(), SelectedDay = new DateTime(2020, 01, 01) };
            var handler = new GetSpotsAvailablePerHourQueryHandler(moq.Object, mockLocationService.Object);

            var result = await handler.Handle(request, CancellationToken.None);

            for (int i = 0; i < 24; i++)
            {
                if (i == 10)
                {
                    Assert.Equal(11, result.First(x => x.Hour == i).SpotsReserved);
                    continue;
                }

                if (i == 12)
                {
                    Assert.Equal(1, result.First(x => x.Hour == i).SpotsReserved);
                    continue;
                }

                Assert.Equal(0, result.First(x => x.Hour == i).SpotsReserved);
            }
        }

        [Fact]
        public async Task Should_return_14_by_hour_when_2_access_requests_today()
        {
            var moq = new Mock<IAccessRequestService>();
            moq.Setup(x => x.GetApprovedOrPendingAccessRequestsByFloor(It.IsAny<Guid>(), It.IsAny<DateTime>()))
                .ReturnsAsync(() => new AccessRequest[]
                {
                    new AccessRequest
                    {
                        StartTime = new DateTime(2020, 01, 01).AddHours(10),
                        EndTime = new DateTime(2020, 01, 01).AddHours(10).AddMinutes(59),
                        Visitors = Enumerable.Range(0, 10).Select(x => new Contact { }).ToList()
                    },
                    new AccessRequest
                    {
                        StartTime = new DateTime(2020, 01, 01).AddHours(10),
                        EndTime = new DateTime(2020, 01, 01).AddHours(10).AddMinutes(59),
                        Visitors = Enumerable.Range(0, 2).Select(x => new Contact { }).ToList()
                    },
                    new AccessRequest
                    {
                        StartTime = new DateTime(2020, 01, 02).AddHours(12),
                        EndTime = new DateTime(2020, 01, 02).AddHours(12).AddMinutes(59)
                    }
                });

            var mockLocationService = new Mock<ILocationService>();
            mockLocationService.Setup(x => x.GetCapacityByFloorAsync(It.IsAny<Guid>()))
                .ReturnsAsync(20);

            var request = new GetSpotsAvailablePerHourQuery { FloorId = Guid.NewGuid(), SelectedDay = new DateTime(2020, 01, 01) };
            var handler = new GetSpotsAvailablePerHourQueryHandler(moq.Object, mockLocationService.Object);

            var result = await handler.Handle(request, CancellationToken.None);

            for (int i = 0; i < 24; i++)
            {
                if (i == 10)
                {
                    Assert.Equal(14, result.First(x => x.Hour == i).SpotsReserved);
                    continue;
                }

                Assert.Equal(0, result.First(x => x.Hour == i).SpotsReserved);
            }
        }
    }
}
