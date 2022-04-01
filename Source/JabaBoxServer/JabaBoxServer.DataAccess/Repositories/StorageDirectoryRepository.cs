using JabaBox.Core.Domain.Entities;
using JabaBox.Core.RepositoryAbstractions;

namespace JabaBoxServer.DataAccess.Repositories;

public class StorageDirectoryRepository : IStorageDirectoryRepository
{
    public StorageDirectory? FindDirectory(AccountInfo account, string name)
    {
        throw new NotImplementedException();
    }

    public StorageDirectory UpdateStorageDirectory(StorageDirectory directory)
    {
        throw new NotImplementedException();
    }

    public StorageDirectory CreateDirectory(StorageDirectory directory)
    {
        throw new NotImplementedException();
    }

    public void DeleteDirectory(StorageDirectory directory)
    {
        throw new NotImplementedException();
    }
}