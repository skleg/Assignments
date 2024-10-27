using CloudSales.Core.Entities;
using CloudSales.Core.Interfaces;

namespace CloudSales.Api.Services;

public class CloudServiceMock : ICloudService
{
    private static readonly IEnumerable<Service> _services = [
        new Service { ServiceId = 1, ServiceName = "Microsoft Office 365", Price = 100 },
        new Service { ServiceId = 2, ServiceName = "Visual Studio 2022", Price = 120 },
        new Service { ServiceId = 3, ServiceName = "Microsoft Outlook", Price = 59 }
    ];

    public Task<Service?> GetServiceAsync(int serviceId, CancellationToken ct = default)
    {
        return Task.FromResult(_services.FirstOrDefault(x => x.ServiceId == serviceId));
    }

    public Task<List<Service>> GetServicesAsync(CancellationToken ct = default)
    {
        return Task.FromResult(_services.ToList());
    }
}