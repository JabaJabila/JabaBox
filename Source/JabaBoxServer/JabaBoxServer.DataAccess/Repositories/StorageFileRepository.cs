using JabaBox.Core.Domain.Entities;
using JabaBox.Core.RepositoryAbstractions;

namespace JabaBoxServer.DataAccess.Repositories;

public class StorageFileRepository : IStorageFileRepository
{
    public StorageFile FindFile(AccountInfo account, StorageDirectory directory, string name)
    {
        throw new NotImplementedException();
    }

    public StorageFile AddFile(StorageFile storageFile, byte[] data, StorageDirectory directory)
    {
        throw new NotImplementedException();
    }

    public StorageFile UpdateFile(StorageFile file)
    {
        throw new NotImplementedException();
    }

    public void DeleteFile(StorageFile file)
    {
        throw new NotImplementedException();
    }
}