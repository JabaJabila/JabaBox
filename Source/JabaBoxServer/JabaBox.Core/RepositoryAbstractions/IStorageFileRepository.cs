using JabaBox.Core.Domain.Entities;

namespace JabaBox.Core.RepositoryAbstractions;

public interface IStorageFileRepository
{
    StorageFile? FindFile(AccountInfo account, StorageDirectory directory, string name);
    StorageFile AddFile(StorageFile storageFile, byte[] data, StorageDirectory directory); 
    StorageFile UpdateFile(StorageFile file);
    void DeleteFile(StorageFile file);
    byte[] GetFileData(AccountInfo account, StorageDirectory directory, StorageFile file);
}