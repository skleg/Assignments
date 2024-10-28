
using System.Net.Http.Json;
using AutoFixture;
using CloudSales.Api.Contracts;
using CloudSales.Core.Entities;
using CloudSales.Persistence.Database;
using FluentAssertions;

namespace CloudSales.Integration.Tests;

public class AccountTests: IClassFixture<IntegrationTestFactory>, IAsyncLifetime
{
    private readonly Fixture _fixture = new();
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

        // _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
        //     .ForEach(b => _fixture.Behaviors.Remove(b));
        // _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
    }

    [Fact]
    public async Task GetAccount_ShouldReturnAnAccount()
    {
        var expected = _factory.CreateAccount();
        //var expected = _fixture.Create<Account>();
        await _dbContext.AddAsync(expected);
        await _dbContext.SaveChangesAsync();
        
        var actual = await _client.GetFromJsonAsync<AccountResponse>($"/api/accounts/{expected.AccountId}");

        actual.Should().NotBeNull();
        actual.ShouldBeEquivalentTo(expected);
    }

    public Task InitializeAsync() => Task.CompletedTask;
    public Task DisposeAsync() => _resetDatabase();
}