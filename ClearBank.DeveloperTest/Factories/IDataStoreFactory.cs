using ClearBank.DeveloperTest.Data;

namespace ClearBank.DeveloperTest.Factories;

public interface IDataStoreFactory
{
    IDataStore GetDataStore(string dataStoreType);
}
