using ClearBank.DeveloperTest.Types;

namespace ClearBank.DeveloperTest.Validators.PaymentSchemeValidators;

public class ChapsPaymentsSchemeValidator : IValidator
{
    public bool Validate(Account account, decimal requestAmount = 0) => account.AllowedPaymentSchemes.HasFlag(AllowedPaymentSchemes.Chaps) &&
                account.Status == AccountStatus.Live;
}
