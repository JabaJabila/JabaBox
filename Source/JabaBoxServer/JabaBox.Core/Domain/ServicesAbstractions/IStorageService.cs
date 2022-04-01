using JabaBox.Core.Domain.Entities;
using JabaBox.Core.Domain.Enums;

namespace JabaBox.Core.Domain.ServicesAbstractions;

public interface IStorageService
{
    StorageDirectory CreateDirectory(AccountInfo account, string name);
    StorageDirectory RenameDirectory(AccountInfo account, StorageDirectory directory, string newName);
    StorageDirectory DeleteDirectory(AccountInfo account, StorageDirectory directory);
    
    StorageDirectory AddFile(AccountInfo account, StorageDirectory directory, FileState state, string name, byte[] data);
    StorageDirectory RenameFile(AccountInfo account, StorageDirectory directory, StorageFile file,  string newName);
    StorageDirectory DeleteFile(AccountInfo account, StorageDirectory directory, StorageFile file);
}