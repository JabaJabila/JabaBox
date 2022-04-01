using JabaBox.Core.Domain.Entities;

namespace JabaBox.Core.RepositoryAbstractions;

public interface IAccountInfoRepository
{
    Task<bool> CheckIfLoginExists(string login);
    Task<AccountInfo> SaveAccountInfo(AccountInfo account);
    Task<AccountInfo?> FindAccountByLogin(string login);
    Task<AccountInfo?> FindAccountById(Guid id);
    Task<AccountInfo> UpdateAccountInfo(AccountInfo account);
}