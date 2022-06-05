using AutoFixture;
using ClearBank.DeveloperTest.Data;
using ClearBank.DeveloperTest.Services;
using ClearBank.DeveloperTest.Tests.Helpers;
using ClearBank.DeveloperTest.Types;
using Microsoft.Extensions.Configuration;
using Moq;
using System.Collections.Generic;
using Xunit;


namespace ClearBank.DeveloperTest.Tests.ServicesTests;

public class PaymentServiceWithStoreTypeBackupDisabledTests
{
    private readonly IConfiguration configuration;
    private readonly Mock<IAccountDataStore> mockAccountDataStore = new Mock<IAccountDataStore>();
    private readonly Mock<IBackupAccountDataStore> mockBackupAccountDataStore = new Mock<IBackupAccountDataStore>();
    private readonly Fixture fixture;
    private IPaymentService paymentService;

    public PaymentServiceWithStoreTypeBackupDisabledTests()
    {
        configuration = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string> { }).Build();
        paymentService = new PaymentService(mockAccountDataStore.Object, mockBackupAccountDataStore.Object, configuration);
        fixture = new Fixture();
    }

    [Fact]
    public void MakePayment_ForRequest_ReturnsSuccess_False_IfAccountNotFound()
    {
        var paymentRequest = fixture.Create<MakePaymentRequest>();

        var result = paymentService.MakePayment(paymentRequest);

        Assert.NotNull(result);
        Assert.False(result.Success);
        mockAccountDataStore.Verify(x => x.UpdateAccount(It.IsAny<Account>()), Times.Never);
    }

    [Fact]
    public void MakePayment_ForRequest_ReturnsSuccess_False_IfRequestedAmountIsGreaterThanAccountBalance()
    {
        var paymentRequest = fixture.Create<MakePaymentRequest>();

        var account = fixture.Create<Account>();
        account.Balance = 0;

        mockAccountDataStore
            .Setup(x => x.GetAccount(It.Is<string>(x => x == paymentRequest.DebtorAccountNumber)))
            .Returns(account);

        var result = paymentService.MakePayment(paymentRequest);

        Assert.NotNull(result);
        Assert.False(result.Success);
        mockAccountDataStore.Verify(x => x.UpdateAccount(It.IsAny<Account>()), Times.Never);
    }

    [Theory]
    [InlineData(PaymentScheme.Bacs)]
    [InlineData(PaymentScheme.FasterPayments)]
    [InlineData(PaymentScheme.Chaps)]
    public void MakePayment_ForRequest_ReturnsSuccess_False_IfAccountStatusNotLive(PaymentScheme requestedPaymentScheme)
    {
        var paymentRequest = fixture.Create<MakePaymentRequest>();
        paymentRequest.PaymentScheme = requestedPaymentScheme;

        var account = fixture.Create<Account>();
        account.AccountNumber = paymentRequest.DebtorAccountNumber;
        account.AllowedPaymentSchemes = PaymentServiceTestHelper.GetAllowedPaymentSchemesForRequest(requestedPaymentScheme);
        account.Status = AccountStatus.Disabled;
        account.Balance += paymentRequest.Amount;

        mockAccountDataStore
            .Setup(x => x.GetAccount(It.Is<string>(x => x == paymentRequest.DebtorAccountNumber)))
            .Returns(account);

        var result = paymentService.MakePayment(paymentRequest);

        Assert.NotNull(result);
        Assert.False(result.Success);
        mockAccountDataStore.Verify(x => x.UpdateAccount(It.IsAny<Account>()), Times.Never);
    }

    [Theory]
    [InlineData(PaymentScheme.Bacs)]
    [InlineData(PaymentScheme.FasterPayments)]
    [InlineData(PaymentScheme.Chaps)]
    public void MakePayment_ForRequest_ReturnsSuccess_False_IfRquestedPaymentSchemeNotInAllowedPaymentScheme(PaymentScheme requestedPaymentScheme)
    {
        var paymentRequest = fixture.Create<MakePaymentRequest>();
        paymentRequest.PaymentScheme = requestedPaymentScheme;

        var account = fixture.Create<Account>();
        account.AccountNumber = paymentRequest.DebtorAccountNumber;
        account.AllowedPaymentSchemes = 0;
        account.Status = AccountStatus.Live;
        account.Balance += paymentRequest.Amount;

        mockAccountDataStore
            .Setup(x => x.GetAccount(It.Is<string>(x => x == paymentRequest.DebtorAccountNumber)))
            .Returns(account);

        var result = paymentService.MakePayment(paymentRequest);

        Assert.NotNull(result);
        Assert.False(result.Success);
        mockAccountDataStore.Verify(x => x.UpdateAccount(It.IsAny<Account>()), Times.Never);
    }

    [Theory]
    [InlineData(PaymentScheme.Bacs)]
    [InlineData(PaymentScheme.FasterPayments)]
    [InlineData(PaymentScheme.Chaps)]
    public void MakePayment_ForRequest_ReturnsSuccess_True(PaymentScheme requestedPaymentScheme)
    {
        var paymentRequest = fixture.Create<MakePaymentRequest>();
        paymentRequest.PaymentScheme = requestedPaymentScheme;

        var account = fixture.Create<Account>();
        account.AccountNumber = paymentRequest.DebtorAccountNumber;
        account.AllowedPaymentSchemes = PaymentServiceTestHelper.GetAllowedPaymentSchemesForRequest(requestedPaymentScheme);
        account.Status = AccountStatus.Live;
        account.Balance += paymentRequest.Amount;

        mockAccountDataStore
            .Setup(x => x.GetAccount(It.Is<string>(x => x == paymentRequest.DebtorAccountNumber)))
            .Returns(account);

        var result = paymentService.MakePayment(paymentRequest);

        Assert.NotNull(result);
        Assert.True(result.Success);
        mockAccountDataStore.Verify(x => x.UpdateAccount(It.IsAny<Account>()), Times.Once);
    }

    [Theory]
    [InlineData(PaymentScheme.Bacs)]
    [InlineData(PaymentScheme.FasterPayments)]
    [InlineData(PaymentScheme.Chaps)]
    public void MakePayment_ForRequest_ReturnsSuccess_True_WithCorrectUpdatedBalance(PaymentScheme requestedPaymentScheme)
    {
        var paymentRequest = fixture.Create<MakePaymentRequest>();
        paymentRequest.PaymentScheme = requestedPaymentScheme;

        var account = fixture.Create<Account>();
        account.AccountNumber = paymentRequest.DebtorAccountNumber;
        account.AllowedPaymentSchemes = PaymentServiceTestHelper.GetAllowedPaymentSchemesForRequest(requestedPaymentScheme);
        account.Status = AccountStatus.Live;
        account.Balance += paymentRequest.Amount;

        mockAccountDataStore
            .Setup(x => x.GetAccount(It.Is<string>(x => x == paymentRequest.DebtorAccountNumber)))
            .Returns(account);

        var result = paymentService.MakePayment(paymentRequest);

        Assert.NotNull(result);
        Assert.True(result.Success);
        account.Balance -= paymentRequest.Amount;
        mockAccountDataStore.Verify(x => x.UpdateAccount(It.Is<Account>(x => x == account)), Times.Once);
    }

    [Fact]
    public void MakePayment_ForRequest_ReturnsSuccess_True_ShouldReturnLateOnBackupStoreDisabled()
    {
        var paymentRequest = fixture.Create<MakePaymentRequest>();

        var account = fixture.Create<Account>();
        account.AccountNumber = paymentRequest.DebtorAccountNumber;
        account.AllowedPaymentSchemes = PaymentServiceTestHelper.GetAllowedPaymentSchemesForRequest(paymentRequest.PaymentScheme);
        account.Status = AccountStatus.Live;
        account.Balance += paymentRequest.Amount;

        mockAccountDataStore
            .Setup(x => x.GetAccount(It.Is<string>(x => x == paymentRequest.DebtorAccountNumber)))
            .Returns(account);

        var result = paymentService.MakePayment(paymentRequest);

        Assert.NotNull(result);
        Assert.True(result.Success);
        mockBackupAccountDataStore.Verify(x => x.UpdateAccount(It.IsAny<Account>()), Times.Never);
    }
}
