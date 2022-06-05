using ClearBank.DeveloperTest.Types;

namespace ClearBank.DeveloperTest.Validators;

public interface IPaymentsValidator
{
    bool Validate(PaymentScheme paymentScheme, Account account, decimal amount = 0);
}
