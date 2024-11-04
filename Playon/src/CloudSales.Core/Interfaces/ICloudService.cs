using CloudSales.Core.Dtos;

namespace CloudSales.Core.Interfaces;

public interface ICloudService
{
    Task<List<ServiceDto>> GetServicesAsync(CancellationToken ct = default);
    Task<ServiceDto?> GetServiceAsync(int serviceId, CancellationToken ct = default);
    Task<PurchaseReceiptDto> CreateSubscriptionAsync(CreateSubscriptionRequest request, CancellationToken ct = default);
    Task<PurchaseReceiptDto> UpdateSubscriptionAsync(UpdateSubscriptionRequest request, CancellationToken ct = default);
    Task CancelSubscriptionAsync(CancelSubscriptionRequest request, CancellationToken ct = default);
}
