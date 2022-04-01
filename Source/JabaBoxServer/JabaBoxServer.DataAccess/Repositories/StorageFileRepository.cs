using JabaBox.Core.Domain.Entities;
using JabaBox.Core.RepositoryAbstractions;

namespace JabaBoxServer.DataAccess.Repositories;

public class StorageFileRepository : IStorageFileRepository
{
    public async Task<StorageFile> FindFile(AccountInfo account, StorageDirectory directory, string name)
    {
        throw new NotImplementedException();
    }

    public async Task<StorageFile> AddFile(StorageFile storageFile, byte[] data, StorageDirectory directory)
    {
        throw new NotImplementedException();
    }

    public async Task<StorageFile> UpdateFile(StorageFile file)
    {
        throw new NotImplementedException();
    }

    public async void DeleteFile(StorageFile file)
    {
        throw new NotImplementedException();
    }
}