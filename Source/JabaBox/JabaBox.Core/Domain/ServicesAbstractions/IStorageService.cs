using JabaBox.Core.Domain.Entities;
using JabaBox.Core.Domain.Enums;

namespace JabaBox.Core.Domain.ServicesAbstractions;

public interface IStorageService
{
    BaseDirectory GetBaseDirectory(AccountInfo account);
    StorageDirectory? FindDirectory(AccountInfo account, string name);
    StorageFile? FindFile(AccountInfo account, StorageDirectory directory, string name);
    
    StorageDirectory CreateDirectory(AccountInfo account, string name);
    StorageDirectory RenameDirectory(AccountInfo account, StorageDirectory directory, string newName);
    void DeleteDirectory(AccountInfo account, StorageDirectory directory);
    
    StorageFile AddFile(AccountInfo account, StorageDirectory directory, FileState state, string name, byte[] data);
    StorageFile RenameFile(AccountInfo account, StorageDirectory directory, StorageFile file,  string newName);
    void DeleteFile(AccountInfo account, StorageDirectory directory, StorageFile file);
    long BytesAvailable(AccountInfo account);
    byte[] GetFileData(AccountInfo account, StorageDirectory storage, StorageFile file);
}