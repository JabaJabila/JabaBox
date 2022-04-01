using JabaBox.Core.Domain.Entities;

namespace JabaBox.Core.RepositoryAbstractions;

public interface IAccountInfoRepository
{
    bool CheckIfLoginExists(string login);
    AccountInfo SaveAccountInfo(AccountInfo accountInfo);
    AccountInfo? FindAccountByLogin(string login);
    AccountInfo? FindAccountById(Guid id);
    AccountInfo UpdateAccountInfo(AccountInfo account);
}