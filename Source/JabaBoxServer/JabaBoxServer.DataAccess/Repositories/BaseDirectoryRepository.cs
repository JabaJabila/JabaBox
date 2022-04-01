using JabaBox.Core.Domain.Entities;
using JabaBox.Core.RepositoryAbstractions;

namespace JabaBoxServer.DataAccess.Repositories;

public class BaseDirectoryRepository : IBaseDirectoryRepository
{
    public void CreateBaseDirectory(Guid accountId)
    {
        throw new NotImplementedException();
    }

    public BaseDirectory GetBaseDirectoryById(Guid accountId)
    {
        throw new NotImplementedException();
    }

    public BaseDirectory UpdateBaseDirectory(BaseDirectory directory)
    {
        throw new NotImplementedException();
    }
}