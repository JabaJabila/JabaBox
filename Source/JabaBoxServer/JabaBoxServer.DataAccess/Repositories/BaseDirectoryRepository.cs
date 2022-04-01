using JabaBox.Core.Domain.Entities;
using JabaBox.Core.RepositoryAbstractions;

namespace JabaBoxServer.DataAccess.Repositories;

public class BaseDirectoryRepository : IBaseDirectoryRepository
{
    public async void CreateBaseDirectory(Guid accountId)
    {
        throw new NotImplementedException();
    }

    public async Task<BaseDirectory> GetBaseDirectoryById(Guid accountId)
    {
        throw new NotImplementedException();
    }

    public async Task<BaseDirectory> UpdateBaseDirectory(BaseDirectory directory)
    {
        throw new NotImplementedException();
    }
}