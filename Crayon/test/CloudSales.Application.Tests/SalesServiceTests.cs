using CloudSales.Application.Services;
using CloudSales.Core.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;
using ErrorOr;
using FluentAssertions;
using CloudSales.Core.Errors;

namespace CloudSales.Application.Tests;

public class SalesServiceTests
{
    private readonly SalesService _sut;
    private readonly Mock<ICloudService> _cloudMock = new();
    private readonly Mock<ISalesRepository> _repositoryMock = new();
    private readonly Mock<ILogger<SalesService>> _loggerMock = new();

    public SalesServiceTests()
    {
        _sut = new SalesService(_repositoryMock.Object, _cloudMock.Object, _loggerMock.Object);
    }

    [Theory]
    [InlineData(0, 0)]
    [InlineData(1, 0)]
    [InlineData(0, 10)]
    [InlineData(1, -1)]
    [InlineData(-1, 10)]
    [InlineData(-1, -1)]
    [InlineData(1, 1)]
    public async void GetAccountsAsync_ShouldReturnInvalidPagination_WhenPaginationIsNotValid(int pageNo, int pageSize)
    {
        // Arrange
        int customerId = 1;

        // Act
        var result = await _sut.GetAccountsAsync(customerId, pageNo, pageSize);

        // Assert
        result.FirstError.Should().NotBeNull();
        result.FirstError.Code.Should().Be(CommonErrors.InvalidPagination.Code);
    }
}