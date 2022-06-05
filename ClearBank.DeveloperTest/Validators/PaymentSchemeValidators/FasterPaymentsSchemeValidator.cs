using ClearBank.DeveloperTest.Types;

namespace ClearBank.DeveloperTest.Validators.PaymentSchemeValidators;

public class FasterPaymentsSchemeValidator : IValidator
{
    public bool Validate(Account account, decimal requestAmount = 0) => account.AllowedPaymentSchemes.HasFlag(AllowedPaymentSchemes.FasterPayments) &&
                requestAmount != 0 && account.Balance > requestAmount;
}
