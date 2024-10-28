
using System.Net.Http.Json;
using CloudSales.Api.Contracts;
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

    public Task InitializeAsync() => Task.CompletedTask;
    public Task DisposeAsync() => _resetDatabase();
}