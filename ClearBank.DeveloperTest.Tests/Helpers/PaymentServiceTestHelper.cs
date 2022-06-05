using ClearBank.DeveloperTest.Types;

namespace ClearBank.DeveloperTest.Tests.Helpers;

public static class PaymentServiceTestHelper
{
    public static AllowedPaymentSchemes GetAllowedPaymentSchemesForRequest(PaymentScheme requestedPaymentScheme)
    {
        return requestedPaymentScheme == PaymentScheme.Bacs ? AllowedPaymentSchemes.Bacs :
            requestedPaymentScheme == PaymentScheme.FasterPayments ? AllowedPaymentSchemes.FasterPayments :
           requestedPaymentScheme == PaymentScheme.Chaps ? AllowedPaymentSchemes.Chaps : 0;
    }
}
