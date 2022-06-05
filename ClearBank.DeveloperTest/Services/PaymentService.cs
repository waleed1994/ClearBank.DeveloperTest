using ClearBank.DeveloperTest.Types;
using ClearBank.DeveloperTest.Validators;

namespace ClearBank.DeveloperTest.Services;

public class PaymentService : IPaymentService
{
    private readonly IAccountService accountService;
    private readonly IPaymentsValidator paymentsValidator;

    public PaymentService(IAccountService accountService, IPaymentsValidator paymentsValidator)
    {
        this.accountService = accountService;
        this.paymentsValidator = paymentsValidator;
    }

    public MakePaymentResult MakePayment(MakePaymentRequest request)
    {
        var account = accountService.GetAccount(request.DebtorAccountNumber);

        if (account == null) return new MakePaymentResult { Success = false };

        var result = new MakePaymentResult 
        { 
            Success = paymentsValidator.Validate(request.PaymentScheme, account, request.Amount)
        };
        
        if (!result.Success) return result;

        account.Balance -= request.Amount;

        accountService.UpdateAccount(account);

        return result;
    }
}
