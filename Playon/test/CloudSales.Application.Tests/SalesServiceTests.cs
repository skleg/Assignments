using CloudSales.Application.Services;
using CloudSales.Core.Interfaces;
using CloudSales.Core.Errors;
using FluentAssertions;
using Moq;
using Microsoft.Extensions.Logging;
using CloudSales.Core.Shared;
using CloudSales.Core.Entities;
using CloudSales.Core.Dtos;
using ErrorOr;

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

#region GetAccountsAsync

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

        _repositoryMock.Setup(x => x.GetCustomerAsync(customerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(null as Customer);

        // Act
        var result = await _sut.GetAccountsAsync(customerId, pageNo, pageSize);

        // Assert
        result.FirstError.Should().NotBeNull();
        result.FirstError.Code.Should().Be(CustomerErrors.NotFound.Code);
    }

#endregion
#region GetAccountAsync

    [Fact]
    public async void GetAccountAsync_ShouldReturnAccountNotFound_WhenAccountDoesNotExist()
    {
        // Arrange
        var accountId = 1;

        _repositoryMock.Setup(x => x.GetAccountAsync(accountId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(null as Account);

        // Act
        var result = await _sut.GetAccountAsync(accountId);

        // Assert
        result.FirstError.Should().NotBeNull();
        result.FirstError.Code.Should().Be(AccountErrors.NotFound.Code);
    }

#endregion
#region GetLicensesAsync

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

        _repositoryMock.Setup(x => x.GetAccountAsync(accountId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(null as Account);

        // Act
        var result = await _sut.GetLicensesAsync(accountId, pageNo, pageSize);

        // Assert
        result.FirstError.Should().NotBeNull();
        result.FirstError.Code.Should().Be(AccountErrors.NotFound.Code);
    }

#endregion
#region GetLicenseAsync

    [Fact]
    public async void GetLicenseAsync_ShouldReturnLicenseNotFound_WhenLicenseDoesNotExist()
    {
        // Arrange
        _repositoryMock.Setup(x => x.GetLicenseAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(null as License);

        // Act
        var result = await _sut.GetLicenseAsync(It.IsAny<int>(), It.IsAny<int>());

        // Assert
        result.FirstError.Should().NotBeNull();
        result.FirstError.Code.Should().Be(LicenseErrors.NotFound.Code);
    }

#endregion
#region ExtendLicenseAsync

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

        _repositoryMock.Setup(x => x.GetLicenseAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
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

        _repositoryMock.Setup(x => x.GetLicenseAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(CreateLicense());
        _repositoryMock.Setup(x => x.GetAccountAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
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

#endregion
#region CancelLicenseAsync

    [Fact]
    public async void CancelLicenseAsync_ShouldReturnLicenseNotFound_WhenLicenseDoesNotExist()
    {
        // Arrange
        _repositoryMock.Setup(x => x.GetLicenseAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(null as License);

        // Act
        var result = await _sut.CancelLicenseAsync(It.IsAny<int>(), It.IsAny<int>());

        // Assert
        result.FirstError.Should().NotBeNull();
        result.FirstError.Code.Should().Be(LicenseErrors.NotFound.Code);
    }

    [Fact]
    public async void CancelLicenseAsync_ShouldReturnAccountNotFound_WhenAccountDoesNotExist()
    {
        // Arrange
        _repositoryMock.Setup(x => x.GetLicenseAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(CreateLicense());
        _repositoryMock.Setup(x => x.GetAccountAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(null as Account);
        
        // Act
        var result = await _sut.CancelLicenseAsync(It.IsAny<int>(), It.IsAny<int>());

        // Assert
        result.FirstError.Should().NotBeNull();
        result.FirstError.Code.Should().Be(AccountErrors.NotFound.Code);
    }

    [Fact]
    public async void CancelLicenseAsync_ShouldReturnError_WhenCloudServiceThrowsAnException()
    {
        // Arrange
        var account = CreateAccount();
        var license = CreateLicense();

        _repositoryMock.Setup(x => x.GetLicenseAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(license);
        _repositoryMock.Setup(x => x.GetAccountAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(account);
        _cloudMock.Setup(x => x.CancelSubscriptionAsync(It.IsAny<CancelSubscriptionRequest>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception());
        
        // Act
        var result = await _sut.CancelLicenseAsync(It.IsAny<int>(), It.IsAny<int>());

        // Assert
        result.FirstError.Should().NotBeNull();
        result.FirstError.Code.Should().Be(LicenseErrors.FailedToUpdateSubscription.Code);
    }

    [Fact]
    public async void CancelLicenseAsync_ShouldUpdateLicense()
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
        var result = await _sut.CancelLicenseAsync(It.IsAny<int>(), It.IsAny<int>());

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().Be(Result.Deleted);

        _cloudMock.Verify(x => x.CancelSubscriptionAsync(It.IsAny<CancelSubscriptionRequest>(), It.IsAny<CancellationToken>()), Times.Once);
        _repositoryMock.Verify(x => x.DeleteLicenseAsync(It.IsAny<License>(), It.IsAny<CancellationToken>()), Times.Once);
    }

#endregion
#region UpdateNumberOfLicensesAsync

    [Fact]
    public async void UpdateNumberOfLicensesAsync_ShouldUpdateNumberOfLicenses()
    {
        // Arrange
        var account = CreateAccount();
        var license = CreateLicense();
        int numberOfLicenses = 2;
        var dto = new UpdateSubscriptionRequest(account.UserName, license.ServiceId, numberOfLicenses, license.ExpiryDate);

        _repositoryMock.Setup(x => x.GetLicenseAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(license);
        _repositoryMock.Setup(x => x.GetAccountAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(account);
        
        // Act
        var result = await _sut.UpdateNumberOfLicensesAsync(It.IsAny<int>(), It.IsAny<int>(), numberOfLicenses, It.IsAny<CancellationToken>());

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Quantity.Should().Be(numberOfLicenses);

        _cloudMock.Verify(x => x.UpdateSubscriptionAsync(dto, It.IsAny<CancellationToken>()), Times.Once);
        _repositoryMock.Verify(x => x.UpdateLicenseAsync(It.IsAny<License>(), It.IsAny<CancellationToken>()), Times.Once);
    }

#endregion
#region UpdateNumberOfLicensesAsync

    [Fact]
    public async void CreateLicenseAsync_ShouldUpdateNumberOfLicenses()
    {
        // Arrange
        var account = CreateAccount();
        var service = new ServiceDto { ServiceId = 1, ServiceName = "Service", Price = 100 };
        var dto = new CreateLicenseDto(account.AccountId, service.ServiceId, 1, 1);
        var receipt = new PurchaseReceiptDto
        {
            NumberOfLicenses = dto.NumberOfLicenses,
            Price = service.Price,
            ServiceName = service.ServiceName,
            ServiceId = service.ServiceId,
            UserName = account.UserName,
            ValidFrom = DateTime.Today,
            ValidUntil = DateTime.Today.AddMonths(dto.NumberOfMonths),
        };
        var expected = new License { 
            AccountId = account.AccountId, 
            ServiceId = service.ServiceId, 
            ExpiryDate = DateTime.Today.AddMonths(dto.NumberOfMonths), 
            Price = service.Price, 
            Quantity = dto.NumberOfLicenses, 
            ServiceName = service.ServiceName, 
            State = LicenseState.Active 
        };

        _repositoryMock.Setup(x => x.GetAccountAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(account);
        _repositoryMock.Setup(x => x.GetLicenseAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(null as License);
        _cloudMock.Setup(x => x.GetServiceAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(service);
        _cloudMock.Setup(x => x.CreateSubscriptionAsync(It.IsAny<CreateSubscriptionRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(receipt);
        
        // Act
        var result = await _sut.CreateLicenseAsync(dto, It.IsAny<CancellationToken>());

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.ShouldBeEquivalentTo(expected);

        _cloudMock.Verify(x => x.CreateSubscriptionAsync(It.IsAny<CreateSubscriptionRequest>(), It.IsAny<CancellationToken>()), Times.Once);
        _repositoryMock.Verify(x => x.CreateLicenseAsync(It.IsAny<License>(), It.IsAny<CancellationToken>()), Times.Once);
    }

#endregion

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
}