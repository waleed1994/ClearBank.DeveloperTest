using ClearBank.DeveloperTest.Types;
using ClearBank.DeveloperTest.Validators;
using Moq;
using System.Collections.Generic;
using Xunit;

namespace ClearBank.DeveloperTest.Tests.ValidatorTests;

public class PaymentsValidatorTests
{
    private readonly Mock<IValidator> bacsPaymentValidatorMock = new Mock<IValidator>();
    private readonly Mock<IValidator> fasterPaymentsValidatorMock = new Mock<IValidator>();
    private readonly Mock<IValidator> chapsPaymentValidatorMock = new Mock<IValidator>();
    private readonly IPaymentsValidator paymentsValidator;

    public PaymentsValidatorTests()
    {
        paymentsValidator = new PaymentsValidator
        {
            Validators = new Dictionary<PaymentScheme, IValidator>
                {
                    {PaymentScheme.Bacs, bacsPaymentValidatorMock.Object},
                    {PaymentScheme.FasterPayments, fasterPaymentsValidatorMock.Object},
                    {PaymentScheme.Chaps, chapsPaymentValidatorMock.Object}
                }
        };
    }

    [Fact]
    public void Validate_PaymentSchemes_BacsPaymentValidator()
    {
        paymentsValidator.Validate(PaymentScheme.Bacs, new Account());
        bacsPaymentValidatorMock.Verify(x => x.Validate(It.IsAny<Account>(), It.IsAny<decimal>()), Times.Once);
    }

    [Fact]
    public void Validate_PaymentSchemes_FasterPaymentValidator()
    {
        paymentsValidator.Validate(PaymentScheme.FasterPayments, new Account());
        fasterPaymentsValidatorMock.Verify(x => x.Validate(It.IsAny<Account>(), It.IsAny<decimal>()), Times.Once);
    }

    [Fact]
    public void Validate_PaymentSchemes_ChapsPaymentValidator()
    {
        paymentsValidator.Validate(PaymentScheme.Chaps, new Account());
        chapsPaymentValidatorMock.Verify(x => x.Validate(It.IsAny<Account>(), It.IsAny<decimal>()), Times.Once);
    }
}
