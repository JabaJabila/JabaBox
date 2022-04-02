namespace JabaBoxServer.DataAccess.Repositories.FileSystemStorages.Abstractions;

public interface IFileSystemStorageDirectoryStorage
{
    void CreateStorageDirectory(Guid baseDirectoryUserId, Guid directoryId);
    void DeleteStorageDirectory(Guid baseDirectoryUserId, Guid directoryId);
    void CheckStorageDirectory(Guid baseDirectoryUserId, Guid directoryId);
}