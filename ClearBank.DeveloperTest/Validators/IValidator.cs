using ClearBank.DeveloperTest.Types;

namespace ClearBank.DeveloperTest.Validators;

public interface IValidator
{
    bool Validate(Account account, decimal requestAmount = 0);
}
