using CloudSales.Core.Entities;
using CloudSales.Core.Shared;
using ErrorOr;

namespace CloudSales.Core.Interfaces;

public interface ISalesService
{
    Task<ErrorOr<EntityPage<Account>>> GetAccountsAsync(int customerId, int pageNo, int pageSize, CancellationToken ct = default);
    Task<ErrorOr<Account>> GetAccountAsync(int accountId, CancellationToken ct = default);
    Task<ErrorOr<EntityPage<License>>> GetLicensesAsync(int accountId, int pageNo, int pageSize, CancellationToken ct = default);
    Task<ErrorOr<License>> GetLicenseAsync(int accountId, int serviceId, CancellationToken ct = default);
    Task<ErrorOr<License>> ExtendLicenseAsync(int accountId, int serviceId, int withMonths, CancellationToken ct = default);
    Task<ErrorOr<Deleted>> CancelLicenseAsync(int accountId, int serviceId, CancellationToken ct = default);
    Task<ErrorOr<License>> UpdateNumberOfLicensesAsync(int accountId, int serviceId, int numberOfLicenses, CancellationToken ct = default);

}