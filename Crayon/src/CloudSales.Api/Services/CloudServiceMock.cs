using CloudSales.Core.Dtos;
using CloudSales.Core.Interfaces;

namespace CloudSales.Api.Services;

public class CloudServiceMock : ICloudService
{
    private static readonly IEnumerable<ServiceDto> _services = [
        new ServiceDto { ServiceId = 1, ServiceName = "Microsoft Office 365", Price = 100 },
        new ServiceDto { ServiceId = 2, ServiceName = "Visual Studio 2022", Price = 120 },
        new ServiceDto { ServiceId = 3, ServiceName = "Microsoft Outlook", Price = 59 }
    ];

    public Task<ServiceDto?> GetServiceAsync(int serviceId, CancellationToken ct = default)
    {
        return Task.FromResult(_services.FirstOrDefault(x => x.ServiceId == serviceId));
    }

    public Task<List<ServiceDto>> GetServicesAsync(CancellationToken ct = default)
    {
        return Task.FromResult(_services.ToList());
    }

    public Task<PurchaseReceiptDto> PurchaseServiceAsync(PurchaseRequest request, CancellationToken ct = default)
    {
        var service = _services.FirstOrDefault(x => x.ServiceId == request.ServiceId);
        if (service is null)
            throw new InvalidOperationException("Service not found");

        return Task.FromResult(new PurchaseReceiptDto
        {
            ServiceId = service.ServiceId,
            ServiceName = service.ServiceName,
            Price = service.Price,
            UserName = request.UserName,
            NumberOfLicenses = request.NumberOfLicenses,
            ValidFrom = DateTime.UtcNow.Date,
            ValidUntil = DateTime.UtcNow.Date.AddMonths(request.NumberOfMonths),
        });
    }
}