using JabaBox.Core.Domain.Entities;
using JabaBox.Core.RepositoryAbstractions;

namespace JabaBoxServer.DataAccess.Repositories;

public class AccountInfoRepository : IAccountInfoRepository
{
    public async Task<bool> CheckIfLoginExists(string login)
    {
        throw new NotImplementedException();
    }

    public async Task<AccountInfo> SaveAccountInfo(AccountInfo account)
    {
        throw new NotImplementedException();
    }

    public async Task<AccountInfo?> FindAccountByLogin(string login)
    {
        throw new NotImplementedException();
    }

    public async Task<AccountInfo?> FindAccountById(Guid id)
    {
        throw new NotImplementedException();
    }

    public async Task<AccountInfo> UpdateAccountInfo(AccountInfo account)
    {
        throw new NotImplementedException();
    }
}