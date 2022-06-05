using ClearBank.DeveloperTest.Data;
using System;

namespace ClearBank.DeveloperTest.Factories;

public class DataStoreFactory : IDataStoreFactory
{
    private const string BackUp = "Backup";

    public IDataStore GetDataStore(string dataStoreType)
    {
        return string.Equals(dataStoreType, BackUp, StringComparison.CurrentCultureIgnoreCase) ?  new BackupAccountDataStore() : new AccountDataStore();
    }
}
