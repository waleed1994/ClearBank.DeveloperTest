using AutoFixture;
using ClearBank.DeveloperTest.Data;
using ClearBank.DeveloperTest.Factories;
using ClearBank.DeveloperTest.Services;
using ClearBank.DeveloperTest.Types;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Moq;
using System.Collections.Generic;
using Xunit;

namespace ClearBank.DeveloperTest.Tests.ServicesTests;

public class AccountServiceTests
{
    private readonly IConfiguration configuration;
    private readonly Mock<IDataStoreFactory> mockDataStoreFactory = new Mock<IDataStoreFactory>();
    private readonly Mock<IDataStore> mockDataStore = new Mock<IDataStore>();
    private readonly IAccountService accountService;
    private readonly Fixture fixture = new Fixture();

    public AccountServiceTests()
    {
        var inMemorySettings = new Dictionary<string, string> { { "DataStoreType", "Backup" } };
        configuration = new ConfigurationBuilder().AddInMemoryCollection(inMemorySettings).Build();
        mockDataStoreFactory.Setup(x => x.GetDataStore(It.IsAny<string>())).Returns(mockDataStore.Object);
        accountService = new AccountService(mockDataStoreFactory.Object, configuration);
    }

    [Fact]
    public void GetAccount_ExistingAccountNumber_Return_CorrectAccount()
    {
        var accountToFetch = fixture.Create<Account>();
        mockDataStore.Setup(x => x.GetAccount(It.Is<string>(x => x == accountToFetch.AccountNumber))).Returns(accountToFetch);

        var account = accountService.GetAccount(accountToFetch.AccountNumber);

        mockDataStoreFactory.Verify(x => x.GetDataStore(It.IsAny<string>()), Times.Once);
        mockDataStore.Verify(x => x.GetAccount(It.IsAny<string>()), Times.Once);
        account.Should().NotBeNull();
        account.AccountNumber.Should().Be(accountToFetch.AccountNumber, account.AccountNumber);
    }

    [Fact]
    public void GetAccount_Return_Null_IfAccountDoesNotExist()
    {
        mockDataStore.Setup(x => x.GetAccount(It.IsAny<string>())).Returns((Account)null);

        var account = accountService.GetAccount("number");

        mockDataStoreFactory.Verify(x => x.GetDataStore(It.IsAny<string>()), Times.Once);
        mockDataStore.Verify(x => x.GetAccount(It.IsAny<string>()), Times.Once);
        account.Should().BeNull();
    }

    [Fact]
    public void UpdateAccount_UpdateInDataStore()
    {
        var accountToUpdate = fixture.Create<Account>();

        accountService.UpdateAccount(accountToUpdate);

        mockDataStoreFactory.Verify(x => x.GetDataStore(It.IsAny<string>()), Times.Once);
        mockDataStore.Verify(x => x.UpdateAccount(It.Is<Account>(x => x == accountToUpdate)), Times.Once);
    }
}
