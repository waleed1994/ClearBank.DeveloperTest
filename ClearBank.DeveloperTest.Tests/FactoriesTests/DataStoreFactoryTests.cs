using ClearBank.DeveloperTest.Data;
using ClearBank.DeveloperTest.Factories;
using FluentAssertions;
using Xunit;

namespace ClearBank.DeveloperTest.Tests.FactoriesTests;

public class DataStoreFactoryTests
{
    private readonly IDataStoreFactory dataStoreFactory = new DataStoreFactory();

    [Fact]
    public void GetDataStore_WithBackupDatastoreConfig_ToCreate_BackupDataStore()
    {
        var dataStore = dataStoreFactory.GetDataStore("Backup");
        dataStore.Should().BeOfType<BackupAccountDataStore>();
    }

    [Fact]
    public void GetDataStore_WithDataStoreConfig_ToCreate_DataStore()
    {
        var dataStore = dataStoreFactory.GetDataStore("");
        dataStore.Should().BeOfType<AccountDataStore>();
    }

    [Fact]
    public void GetDataStore_WithCaseSensitive_BackupDatastoreConfig_ToCreate_BackupDataStore()
    {
        var dataStore = dataStoreFactory.GetDataStore("backup");
        dataStore.Should().BeOfType<BackupAccountDataStore>();
    }
}
