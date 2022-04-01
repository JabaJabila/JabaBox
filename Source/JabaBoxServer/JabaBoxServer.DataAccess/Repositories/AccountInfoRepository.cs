using JabaBox.Core.Domain.Entities;
using JabaBox.Core.RepositoryAbstractions;

namespace JabaBoxServer.DataAccess.Repositories;

public class AccountInfoRepository : IAccountInfoRepository
{
    public bool CheckIfLoginExists(string login)
    {
        throw new NotImplementedException();
    }

    public AccountInfo SaveAccountInfo(AccountInfo accountInfo)
    {
        throw new NotImplementedException();
    }

    public AccountInfo? FindAccountByLogin(string login)
    {
        throw new NotImplementedException();
    }

    public AccountInfo? FindAccountById(Guid id)
    {
        throw new NotImplementedException();
    }

    public AccountInfo UpdateAccountInfo(AccountInfo account)
    {
        throw new NotImplementedException();
    }
}