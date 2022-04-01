using JabaBox.Core.Domain.Entities;
using JabaBox.Core.RepositoryAbstractions;

namespace JabaBoxServer.DataAccess.Repositories;

public class StorageDirectoryRepository : IStorageDirectoryRepository
{
    public async Task<StorageDirectory?> FindDirectory(AccountInfo account, string name)
    {
        throw new NotImplementedException();
    }

    public async Task<StorageDirectory> UpdateStorageDirectory(StorageDirectory directory)
    {
        throw new NotImplementedException();
    }

    public async Task<StorageDirectory> CreateDirectory(StorageDirectory directory)
    {
        throw new NotImplementedException();
    }

    public async void DeleteDirectory(StorageDirectory directory)
    {
        throw new NotImplementedException();
    }
}