using JabaBox.Core.Domain.Entities;

namespace JabaBox.Core.RepositoryAbstractions;

public interface IStorageFileRepository
{
    Task<StorageFile?> FindFile(AccountInfo account, StorageDirectory directory, string name);
    Task<StorageFile> AddFile(StorageFile storageFile, byte[] data, StorageDirectory directory); 
    Task<StorageFile> UpdateFile(StorageFile file);
    void DeleteFile(StorageFile file);
}