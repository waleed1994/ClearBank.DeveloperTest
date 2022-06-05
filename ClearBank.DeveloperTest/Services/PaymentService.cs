using ClearBank.DeveloperTest.Data;
using ClearBank.DeveloperTest.Types;
using Microsoft.Extensions.Configuration;
using System;

namespace ClearBank.DeveloperTest.Services;

public class PaymentService : IPaymentService
{
    private readonly IAccountDataStore accountDataStore;
    private readonly IBackupAccountDataStore backupAccountDataStore;
    private readonly IConfiguration configuration;
    private bool isStoreTpyeBackup => string.Equals(configuration["DataStoreType"], "Backup", StringComparison.CurrentCultureIgnoreCase);

    public PaymentService(IAccountDataStore accountDataStore, IBackupAccountDataStore backupAccountDataStore, IConfiguration configuration)
    {
        this.accountDataStore = accountDataStore;
        this.backupAccountDataStore = backupAccountDataStore;
        this.configuration = configuration;
    }

    public MakePaymentResult MakePayment(MakePaymentRequest request)
    {
        Account account = GetAccount(request.DebtorAccountNumber);

        if (account == null || account.Balance < request.Amount) return new MakePaymentResult { Success = false };

        var result = new MakePaymentResult
        {
            Success = CanPaymentBeProcessed(account, request.PaymentScheme)
        };

        if (!result.Success)
            return result;

        account.Balance -= request.Amount;

        if (isStoreTpyeBackup)
        {
            backupAccountDataStore.UpdateAccount(account);
            return result;
        }

        accountDataStore.UpdateAccount(account);
        return result;
    }

    private Account GetAccount(string accountNumber)
    {
        return isStoreTpyeBackup ?  
            backupAccountDataStore.GetAccount(accountNumber) : 
            accountDataStore.GetAccount(accountNumber);
    }

    private bool CanPaymentBeProcessed(Account account, PaymentScheme requestedPaymentScheme) => requestedPaymentScheme switch
    {
        PaymentScheme.Bacs => account.AllowedPaymentSchemes.HasFlag(AllowedPaymentSchemes.Bacs) && account.Status == AccountStatus.Live,
        PaymentScheme.FasterPayments => account.AllowedPaymentSchemes.HasFlag(AllowedPaymentSchemes.FasterPayments) && account.Status == AccountStatus.Live,
        PaymentScheme.Chaps => account.AllowedPaymentSchemes.HasFlag(AllowedPaymentSchemes.Chaps) && account.Status == AccountStatus.Live,
        _ => throw new ArgumentOutOfRangeException(nameof(requestedPaymentScheme), $"Not expected requested payment scheme type: {requestedPaymentScheme}")
    };
}
