using CloudSales.Application.Services;
using CloudSales.Core.Interfaces;
using CloudSales.Core.Errors;
using FluentAssertions;
using Moq;
using Microsoft.Extensions.Logging;
using CloudSales.Core.Shared;
using CloudSales.Core.Entities;
using CloudSales.Core.Dtos;

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

    [Fact]
    public async void GetAccountsAsync_ShouldReturnCustomerNotFound_WhenCustomerDoesNotExist()
    {
        // Arrange
        int customerId = 1;
        int pageNo = 1;
        int pageSize = 10;

        _repositoryMock.Setup(x => x.GetCustomerAsync(customerId, CancellationToken.None))
            .ReturnsAsync(null as Customer);

        // Act
        var result = await _sut.GetAccountsAsync(customerId, pageNo, pageSize);

        // Assert
        result.FirstError.Should().NotBeNull();
        result.FirstError.Code.Should().Be(CustomerErrors.NotFound.Code);
    }

    [Fact]
    public async void GetAccountAsync_ShouldReturnAccountNotFound_WhenAccountDoesNotExist()
    {
        // Arrange
        var accountId = 1;

        _repositoryMock.Setup(x => x.GetAccountAsync(accountId, CancellationToken.None))
            .ReturnsAsync(null as Account);

        // Act
        var result = await _sut.GetAccountAsync(accountId);

        // Assert
        result.FirstError.Should().NotBeNull();
        result.FirstError.Code.Should().Be(AccountErrors.NotFound.Code);
    }

    [Theory]
    [InlineData(0, 0)]
    [InlineData(1, 0)]
    [InlineData(0, 10)]
    [InlineData(1, -1)]
    [InlineData(-1, 10)]
    [InlineData(-1, -1)]
    [InlineData(1, 1)]
    public async void GetLicensesAsync_ShouldReturnInvalidPagination_WhenPaginationIsNotValid(int pageNo, int pageSize)
    {
        // Arrange

        // Act
        var result = await _sut.GetLicensesAsync(It.IsAny<int>(), pageNo, pageSize);

        // Assert
        result.FirstError.Should().NotBeNull();
        result.FirstError.Code.Should().Be(CommonErrors.InvalidPagination.Code);
    }

    [Fact]
    public async void GetLicensesAsync_ShouldReturnAccountNotFound_WhenAccountDoesNotExist()
    {
        // Arrange
        int accountId = 1;
        int pageNo = 1;
        int pageSize = 10;

        _repositoryMock.Setup(x => x.GetAccountAsync(accountId, CancellationToken.None))
            .ReturnsAsync(null as Account);

        // Act
        var result = await _sut.GetLicensesAsync(accountId, pageNo, pageSize);

        // Assert
        result.FirstError.Should().NotBeNull();
        result.FirstError.Code.Should().Be(AccountErrors.NotFound.Code);
    }

    [Fact]
    public async void GetLicenseAsync_ShouldReturnLicenseNotFound_WhenLicenseDoesNotExist()
    {
        // Arrange
        _repositoryMock.Setup(x => x.GetLicenseAsync(It.IsAny<int>(), It.IsAny<int>(), CancellationToken.None))
            .ReturnsAsync(null as License);

        // Act
        var result = await _sut.GetLicenseAsync(It.IsAny<int>(), It.IsAny<int>());

        // Assert
        result.FirstError.Should().NotBeNull();
        result.FirstError.Code.Should().Be(LicenseErrors.NotFound.Code);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async void ExtendLicenseAsync_ShouldReturnError_WhenNumberOfMonthsIsNotValid(int numberOfMonths)
    {
        // Arrange

        // Act
        var result = await _sut.ExtendLicenseAsync(It.IsAny<int>(), It.IsAny<int>(), numberOfMonths);

        // Assert
        result.FirstError.Should().NotBeNull();
        result.FirstError.Code.Should().Be(LicenseErrors.InvalidNumberOfMonths.Code);
    }

    [Fact]
    public async void ExtendLicenseAsync_ShouldReturnLicenseNotFound_WhenLicenseDoesNotExist()
    {
        // Arrange
        int numberOfMonths = 1;

        _repositoryMock.Setup(x => x.GetLicenseAsync(It.IsAny<int>(), It.IsAny<int>(), CancellationToken.None))
            .ReturnsAsync(null as License);

        // Act
        var result = await _sut.ExtendLicenseAsync(It.IsAny<int>(), It.IsAny<int>(), numberOfMonths);

        // Assert
        result.FirstError.Should().NotBeNull();
        result.FirstError.Code.Should().Be(LicenseErrors.NotFound.Code);
    }

    [Fact]
    public async void ExtendLicenseAsync_ShouldReturnAccountNotFound_WhenAccountDoesNotExist()
    {
        // Arrange
        int numberOfMonths = 1;

        _repositoryMock.Setup(x => x.GetLicenseAsync(It.IsAny<int>(), It.IsAny<int>(), CancellationToken.None))
            .ReturnsAsync(CreateLicense());
        _repositoryMock.Setup(x => x.GetAccountAsync(It.IsAny<int>(), CancellationToken.None))
            .ReturnsAsync(null as Account);
        
        // Act
        var result = await _sut.ExtendLicenseAsync(It.IsAny<int>(), It.IsAny<int>(), numberOfMonths);

        // Assert
        result.FirstError.Should().NotBeNull();
        result.FirstError.Code.Should().Be(AccountErrors.NotFound.Code);
    }

    [Fact]
    public async void ExtendLicenseAsync_ShouldReturnError_WhenCloudServiceThrowsAnException()
    {
        // Arrange
        var account = CreateAccount();
        var license = CreateLicense();
        int numberOfMonths = 1;

        _repositoryMock.Setup(x => x.GetLicenseAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(license);
        _repositoryMock.Setup(x => x.GetAccountAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(account);
        _cloudMock.Setup(x => x.UpdateSubscriptionAsync(It.IsAny<UpdateSubscriptionRequest>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception());
        
        // Act
        var result = await _sut.ExtendLicenseAsync(It.IsAny<int>(), It.IsAny<int>(), numberOfMonths);

        // Assert
        result.FirstError.Should().NotBeNull();
        result.FirstError.Code.Should().Be(LicenseErrors.FailedToUpdateSubscription.Code);
    }

    [Fact]
    public async void ExtendLicenseAsync_ShouldUpdateLicense()
    {
        // Arrange
        var account = CreateAccount();
        var license = CreateLicense();
        int numberOfMonths = 1;
        var expiryDate = license.ExpiryDate.AddMonths(numberOfMonths);

        _repositoryMock.Setup(x => x.GetLicenseAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(license);
        _repositoryMock.Setup(x => x.GetAccountAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(account);
        
        // Act
        var result = await _sut.ExtendLicenseAsync(It.IsAny<int>(), It.IsAny<int>(), numberOfMonths);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().NotBeNull();
        result.Value.ExpiryDate.Should().Be(expiryDate);
        result.Value.State.Should().Be(LicenseState.Active);

        _cloudMock.Verify(x => x.UpdateSubscriptionAsync(It.IsAny<UpdateSubscriptionRequest>(), It.IsAny<CancellationToken>()), Times.Once);
        _repositoryMock.Verify(x => x.UpdateLicenseAsync(It.IsAny<License>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    private static License CreateLicense()
    {
        return new License
        {
            AccountId = 1,
            ServiceId = 1,
            ExpiryDate = DateTime.UtcNow.AddDays(10),
            Price = 100,
            Quantity = 1,
            ServiceName = "Service",
            State = LicenseState.Active,
        };
    }
    private static Account CreateAccount()
    {
        return new Account
        {
            CustomerId = 1,
            AccountId = 1,
            FirstName = "Bob",
            LastName = "Normal",
            UserName = "user@example.com",
        };
    }

    // _repositoryMock.Setup(x => x.GetAccountsAsync(customerId, new Pagination(pageNo, pageSize), CancellationToken.None))
    //     .ReturnsAsync(new EntityPage<Account>([], 0, pageNo, pageSize));

    // var request = new UpdateSubscriptionRequest(account.UserName, license.ServiceId, license.Quantity, license.ExpiryDate);
}