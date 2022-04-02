using JabaBox.Core.Domain.Entities;
using JabaBoxServer.DataAccess.Repositories.FileSystemStorages.Abstractions;

namespace JabaBoxServer.DataAccess.Repositories.FileSystemStorages.Implementations;

public class FileSystemStorage : IFileSystemStorage
{
    public void CreateBaseDirectory(Guid accountId)
    {
        throw new NotImplementedException();
    }

    public void CheckBaseDirectory(Guid accountId)
    {
        throw new NotImplementedException();
    }

    public void CreateStorageDirectory(Guid baseDirectoryUserId, Guid directoryId)
    {
        throw new NotImplementedException();
    }

    public void DeleteStorageDirectory(Guid baseDirectoryUserId, Guid directoryId)
    {
        throw new NotImplementedException();
    }

    public void CheckStorageDirectory(Guid baseDirectoryUserId, Guid directoryId)
    {
        throw new NotImplementedException();
    }

    public Task<StorageFile?> FindFile(AccountInfo account, StorageDirectory directory, string name)
    {
        throw new NotImplementedException();
    }

    public Task<StorageFile> AddFile(StorageFile storageFile, byte[] data, StorageDirectory directory)
    {
        throw new NotImplementedException();
    }

    public Task<StorageFile> UpdateFile(StorageFile file)
    {
        throw new NotImplementedException();
    }

    public void DeleteFile(StorageFile file)
    {
        throw new NotImplementedException();
    }
}