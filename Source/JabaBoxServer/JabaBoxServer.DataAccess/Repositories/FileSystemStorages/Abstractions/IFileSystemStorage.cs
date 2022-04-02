using JabaBox.Core.RepositoryAbstractions;

namespace JabaBoxServer.DataAccess.Repositories.FileSystemStorages.Abstractions;

public interface IFileSystemStorage : IFileSystemBaseDirectoryStorage, IFileSystemStorageDirectoryStorage, IStorageFileRepository
{
}