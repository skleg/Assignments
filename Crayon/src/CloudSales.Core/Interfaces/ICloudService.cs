using CloudSales.Core.Entities;

namespace CloudSales.Core.Interfaces;

public interface ICloudService
{
    Task<List<Service>> GetServicesAsync(CancellationToken ct = default);
    Task<Service?> GetServiceAsync(int serviceId, CancellationToken ct = default);
}
