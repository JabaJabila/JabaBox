using JabaBox.Core.Domain.Entities;

namespace JabaBox.Core.RepositoryAbstractions;

public interface IStorageDirectoryRepository
{
    Task<StorageDirectory?> FindDirectory(AccountInfo account, string name);
    Task<StorageDirectory> UpdateStorageDirectory(StorageDirectory directory);
    Task<StorageDirectory> CreateDirectory(StorageDirectory directory);
    Task DeleteDirectory(StorageDirectory directory);
}