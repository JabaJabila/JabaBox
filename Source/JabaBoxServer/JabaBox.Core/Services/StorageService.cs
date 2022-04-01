using JabaBox.Core.Domain.Entities;
using JabaBox.Core.Domain.Enums;
using JabaBox.Core.Domain.ServicesAbstractions;

namespace JabaBox.Core.Services;

public class StorageService : IStorageService
{
    public StorageDirectory CreateDirectory(AccountInfo account, string name)
    {
        throw new NotImplementedException();
    }

    public StorageDirectory RenameDirectory(AccountInfo account, StorageDirectory directory, string newName)
    {
        throw new NotImplementedException();
    }

    public StorageDirectory DeleteDirectory(AccountInfo account, StorageDirectory directory)
    {
        throw new NotImplementedException();
    }

    public StorageDirectory AddFile(AccountInfo account, StorageDirectory directory, FileState state, string name, byte[] data)
    {
        throw new NotImplementedException();
    }

    public StorageDirectory RenameFile(AccountInfo account, StorageDirectory directory, StorageFile file, string newName)
    {
        throw new NotImplementedException();
    }

    public StorageDirectory DeleteFile(AccountInfo account, StorageDirectory directory, StorageFile file)
    {
        throw new NotImplementedException();
    }
}