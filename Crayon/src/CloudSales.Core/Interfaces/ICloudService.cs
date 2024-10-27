using CloudSales.Core.Dtos;
using CloudSales.Core.Entities;

namespace CloudSales.Core.Interfaces;

public interface ICloudService
{
    Task<List<ServiceDto>> GetServicesAsync(CancellationToken ct = default);
    Task<ServiceDto?> GetServiceAsync(int serviceId, CancellationToken ct = default);
    Task<PurchaseReceiptDto> PurchaseServiceAsync(PurchaseRequest dto, CancellationToken ct = default);
}
