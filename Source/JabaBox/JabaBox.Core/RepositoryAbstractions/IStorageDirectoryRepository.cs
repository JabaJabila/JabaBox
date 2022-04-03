using JabaBox.Core.Domain.Entities;

namespace JabaBox.Core.RepositoryAbstractions;

public interface IStorageDirectoryRepository
{
    StorageDirectory? FindDirectory(AccountInfo account, string name);
    StorageDirectory UpdateStorageDirectory(StorageDirectory directory);
    StorageDirectory CreateDirectory(StorageDirectory directory);
    void DeleteDirectory(StorageDirectory directory);
}