using System.Net;
using System.Net.Http.Json;
using CloudSales.Api.Contracts;
using CloudSales.Api.Contracts.Requests;
using CloudSales.Persistence.Database;
using FluentAssertions;

namespace CloudSales.Integration.Tests;

public class AccountTests: IClassFixture<IntegrationTestFactory>, IAsyncLifetime
{
    private readonly AppDbContext _dbContext;
    private readonly HttpClient _client;
    private readonly Func<Task> _resetDatabase;
    private readonly IntegrationTestFactory _factory;

    public AccountTests(IntegrationTestFactory factory)
    {
        _dbContext = factory.Db;
        _resetDatabase = factory.ResetDatabase;
        _client = factory.CreateClient();
        _client.DefaultRequestHeaders.Authorization = new("Bearer", factory.GetToken());
        _factory = factory;
    }

    [Fact]
    public async Task GetAccount_ShouldReturnAnAccount()
    {
        // Arrange
        var expected = _factory.Customer.Accounts.First();
        
        // Act
        var actual = await _client.GetFromJsonAsync<AccountResponse>($"api/accounts/{expected.AccountId}");

        // Assert
        actual.Should().NotBeNull();
        actual!.Id.Should().Be(expected.AccountId);
    }

    [Fact]
    public async Task AddLicense_ShouldCreateNewLicense()
    {
        // Arrange
        var account = _factory.Customer.Accounts.First();
        var services = await _factory.CloudService.GetServicesAsync();
        var service = services.First();
        var request = new AddLicenseRequest
        {
            ServiceId = service.ServiceId,
            NumberOfLicenses = 1,
            NumberOfMonths = 1,  
        };
        var expected = new LicenseResponse(service.ServiceId, service.ServiceName, request.NumberOfMonths, DateTime.UtcNow.Date.AddMonths(1), true);

        // Act
        var result = await _client.PostAsJsonAsync($"api/accounts/{account.AccountId}/licenses", request);
        var actual = await result.Content.ReadFromJsonAsync<LicenseResponse>();

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        actual.ShouldBeEquivalentTo(expected);
    }

    public Task InitializeAsync() => Task.CompletedTask;
    public Task DisposeAsync() => _resetDatabase();
}