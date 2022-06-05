using ClearBank.DeveloperTest.Types;
using ClearBank.DeveloperTest.Validators.PaymentSchemeValidators;
using System.Collections.Generic;

namespace ClearBank.DeveloperTest.Validators;

public class PaymentsValidator : IPaymentsValidator
{
    public Dictionary<PaymentScheme, IValidator> Validators { get; set; }

    public PaymentsValidator()
    {
        Validators = new Dictionary<PaymentScheme, IValidator>
        {
            {PaymentScheme.Bacs, new BacsPaymentSchemeValidator()},
            {PaymentScheme.FasterPayments, new FasterPaymentsSchemeValidator()},
            {PaymentScheme.Chaps, new ChapsPaymentsSchemeValidator()}
        };
    }

    public bool Validate(PaymentScheme paymentScheme, Account account, decimal amount = 0)
    {
        return Validators[paymentScheme].Validate(account, amount);
    }
}
