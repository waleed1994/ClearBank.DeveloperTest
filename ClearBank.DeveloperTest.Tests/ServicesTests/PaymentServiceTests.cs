using AutoFixture;
using ClearBank.DeveloperTest.Services;
using ClearBank.DeveloperTest.Types;
using ClearBank.DeveloperTest.Validators;
using FluentAssertions;
using Moq;
using Xunit;

namespace ClearBank.DeveloperTest.Tests.ServicesTests;

public class PaymentServiceTests
{
    private readonly IPaymentsValidator paymentsValidator = new PaymentsValidator();
    private readonly Mock<IAccountService> accountService = new Mock<IAccountService>();
    private readonly Fixture fixture = new Fixture();
    private readonly IPaymentService paymentService;
    public PaymentServiceTests() => paymentService = new PaymentService(accountService.Object, paymentsValidator);


    [Fact]
    public void MakePayment_AccountDoesnotExist_ReturnSuccess_False()
    {
        var paymentReuest = fixture.Create<MakePaymentRequest>();

        var result = paymentService.MakePayment(paymentReuest);

        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        accountService.Verify(x => x.UpdateAccount(It.IsAny<Account>()), Times.Never);
    }

    [Fact]
    public void MakePayment_BacsPaymentScheme_BacsPaymentNotAllowed_ReturnSuccess_False()
    {
        var paymentReuest = fixture.Create<MakePaymentRequest>();
        paymentReuest.PaymentScheme = PaymentScheme.Bacs;
        var account = fixture.Create<Account>();
        account.AccountNumber = paymentReuest.DebtorAccountNumber;
        account.AllowedPaymentSchemes = AllowedPaymentSchemes.Chaps;

        accountService
            .Setup(x => x.GetAccount(It.Is<string>(x => x == paymentReuest.DebtorAccountNumber)))
            .Returns(account);

        var result = paymentService.MakePayment(paymentReuest);

        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        accountService.Verify(x => x.UpdateAccount(It.IsAny<Account>()), Times.Never);
    }

    [Fact]
    public void MakePayment_BacsPaymentScheme_ReturnSuccess_True()
    {
        var paymentReuest = fixture.Create<MakePaymentRequest>();
        paymentReuest.PaymentScheme = PaymentScheme.Bacs;
        var account = fixture.Create<Account>();
        account.AccountNumber = paymentReuest.DebtorAccountNumber;
        account.AllowedPaymentSchemes = AllowedPaymentSchemes.Bacs;
        account.Balance += paymentReuest.Amount;

        accountService
            .Setup(x => x.GetAccount(It.Is<string>(x => x == paymentReuest.DebtorAccountNumber)))
            .Returns(account);

        var result = paymentService.MakePayment(paymentReuest);

        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        accountService.Verify(x => x.UpdateAccount(It.IsAny<Account>()), Times.Once);
    }

    [Fact]
    public void MakePayment_BacsPaymentScheme_On_ReturnSuccess_True_UpdatedTheBalance()
    {
        var paymentReuest = fixture.Create<MakePaymentRequest>();
        paymentReuest.PaymentScheme = PaymentScheme.Bacs;
        var account = fixture.Create<Account>();
        account.AccountNumber = paymentReuest.DebtorAccountNumber;
        account.AllowedPaymentSchemes = AllowedPaymentSchemes.Bacs;
        account.Balance += paymentReuest.Amount;

        accountService
            .Setup(x => x.GetAccount(It.Is<string>(x => x == paymentReuest.DebtorAccountNumber)))
            .Returns(account);

        var result = paymentService.MakePayment(paymentReuest);

        account.Balance -= paymentReuest.Amount;
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        accountService.Verify(x => x.UpdateAccount(It.Is<Account>(x => x == account)), Times.Once);
    }

    [Fact]
    public void MakePayment_FasterPaymentScheme_FasterPaymentNotAllowed_ReturnSuccess_False()
    {
        var paymentReuest = fixture.Create<MakePaymentRequest>();
        paymentReuest.PaymentScheme = PaymentScheme.FasterPayments;
        var account = fixture.Create<Account>();
        account.AccountNumber = paymentReuest.DebtorAccountNumber;
        account.AllowedPaymentSchemes = AllowedPaymentSchemes.Chaps;

        accountService
            .Setup(x => x.GetAccount(It.Is<string>(x => x == paymentReuest.DebtorAccountNumber)))
            .Returns(account);

        var result = paymentService.MakePayment(paymentReuest);

        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        accountService.Verify(x => x.UpdateAccount(It.IsAny<Account>()), Times.Never);
    }

    [Fact]
    public void MakePayment_FasterPaymentScheme_RequestedAmountIsMoreThanBalance_ReturnSuccess_False()
    {
        var paymentReuest = fixture.Create<MakePaymentRequest>();
        paymentReuest.PaymentScheme = PaymentScheme.FasterPayments;
        var account = fixture.Create<Account>();
        account.AccountNumber = paymentReuest.DebtorAccountNumber;
        account.AllowedPaymentSchemes = AllowedPaymentSchemes.FasterPayments;
        account.Balance = 0;

        accountService
            .Setup(x => x.GetAccount(It.Is<string>(x => x == paymentReuest.DebtorAccountNumber)))
            .Returns(account);

        var result = paymentService.MakePayment(paymentReuest);

        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        accountService.Verify(x => x.UpdateAccount(It.IsAny<Account>()), Times.Never);
    }

    [Fact]
    public void MakePayment_FasterPaymentScheme_ReturnSuccess_True()
    {
        var paymentReuest = fixture.Create<MakePaymentRequest>();
        paymentReuest.PaymentScheme = PaymentScheme.FasterPayments;
        var account = fixture.Create<Account>();
        account.AccountNumber = paymentReuest.DebtorAccountNumber;
        account.AllowedPaymentSchemes = AllowedPaymentSchemes.FasterPayments;
        account.Balance += paymentReuest.Amount;

        accountService
            .Setup(x => x.GetAccount(It.Is<string>(x => x == paymentReuest.DebtorAccountNumber)))
            .Returns(account);

        var result = paymentService.MakePayment(paymentReuest);

        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        accountService.Verify(x => x.UpdateAccount(It.IsAny<Account>()), Times.Once);
    }

    [Fact]
    public void MakePayment_FasterPaymentScheme_ReturnSuccess_True_UpdatedTheBalance()
    {
        var paymentReuest = fixture.Create<MakePaymentRequest>();
        paymentReuest.PaymentScheme = PaymentScheme.FasterPayments;
        var account = fixture.Create<Account>();
        account.AccountNumber = paymentReuest.DebtorAccountNumber;
        account.AllowedPaymentSchemes = AllowedPaymentSchemes.FasterPayments;
        account.Balance += paymentReuest.Amount;

        accountService
            .Setup(x => x.GetAccount(It.Is<string>(x => x == paymentReuest.DebtorAccountNumber)))
            .Returns(account);

        var result = paymentService.MakePayment(paymentReuest);

        account.Balance -= paymentReuest.Amount;
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        accountService.Verify(x => x.UpdateAccount(It.Is<Account>(x => x == account)), Times.Once);
    }

    [Fact]
    public void MakePayment_ChapsPaymentScheme_ChapsPaymentNotAllowed_ReturnSuccess_False()
    {
        var paymentReuest = fixture.Create<MakePaymentRequest>();
        paymentReuest.PaymentScheme = PaymentScheme.Chaps;
        var account = fixture.Create<Account>();
        account.AccountNumber = paymentReuest.DebtorAccountNumber;
        account.AllowedPaymentSchemes = AllowedPaymentSchemes.Bacs;

        accountService
            .Setup(x => x.GetAccount(It.Is<string>(x => x == paymentReuest.DebtorAccountNumber)))
            .Returns(account);

        var result = paymentService.MakePayment(paymentReuest);

        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        accountService.Verify(x => x.UpdateAccount(It.IsAny<Account>()), Times.Never);
    }

    [Fact]
    public void MakePayment_ChapsPaymentScheme_AccountStatusIsNotLive_ReturnSuccess_False()
    {
        var paymentReuest = fixture.Create<MakePaymentRequest>();
        paymentReuest.PaymentScheme = PaymentScheme.Chaps;
        var account = fixture.Create<Account>();
        account.AccountNumber = paymentReuest.DebtorAccountNumber;
        account.AllowedPaymentSchemes = AllowedPaymentSchemes.Chaps;
        account.Status = AccountStatus.Disabled;

        accountService
            .Setup(x => x.GetAccount(It.Is<string>(x => x == paymentReuest.DebtorAccountNumber)))
            .Returns(account);

        var result = paymentService.MakePayment(paymentReuest);

        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        accountService.Verify(x => x.UpdateAccount(It.IsAny<Account>()), Times.Never);
    }

    [Fact]
    public void MakePayment_ChapsPaymentScheme_ReturnSuccess_True()
    {
        var paymentReuest = fixture.Create<MakePaymentRequest>();
        paymentReuest.PaymentScheme = PaymentScheme.Chaps;
        var account = fixture.Create<Account>();
        account.AccountNumber = paymentReuest.DebtorAccountNumber;
        account.AllowedPaymentSchemes = AllowedPaymentSchemes.Chaps;
        account.Balance += paymentReuest.Amount;

        accountService
            .Setup(x => x.GetAccount(It.Is<string>(x => x == paymentReuest.DebtorAccountNumber)))
            .Returns(account);

        var result = paymentService.MakePayment(paymentReuest);

        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        accountService.Verify(x => x.UpdateAccount(It.IsAny<Account>()), Times.Once);
    }

    [Fact]
    public void MakePayment_ChapsPaymentScheme_ReturnSuccess_True_UpdatedTheBalance()
    {
        var paymentReuest = fixture.Create<MakePaymentRequest>();
        paymentReuest.PaymentScheme = PaymentScheme.Chaps;
        var account = fixture.Create<Account>();
        account.AccountNumber = paymentReuest.DebtorAccountNumber;
        account.AllowedPaymentSchemes = AllowedPaymentSchemes.Chaps;
        account.Balance += paymentReuest.Amount;

        accountService
            .Setup(x => x.GetAccount(It.Is<string>(x => x == paymentReuest.DebtorAccountNumber)))
            .Returns(account);

        var result = paymentService.MakePayment(paymentReuest);

        account.Balance -= paymentReuest.Amount;
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        accountService.Verify(x => x.UpdateAccount(It.Is<Account>(x => x == account)), Times.Once);
    }
}
