using ClearBank.DeveloperTest.Data;
using ClearBank.DeveloperTest.Factories;
using ClearBank.DeveloperTest.Types;
using Microsoft.Extensions.Configuration;

namespace ClearBank.DeveloperTest.Services;

public class AccountService : IAccountService
{
    private readonly IDataStore dataStore;

    public AccountService(IDataStoreFactory dataStoreFactory, IConfiguration configuration)
    {
        this.dataStore = dataStoreFactory.GetDataStore(configuration["DataStoreType"]);
    }

    public Account GetAccount(string accountNumber)
    {
        return dataStore.GetAccount(accountNumber);
    }

    public void UpdateAccount(Account account)
    {
        dataStore.UpdateAccount(account);
    }
}
