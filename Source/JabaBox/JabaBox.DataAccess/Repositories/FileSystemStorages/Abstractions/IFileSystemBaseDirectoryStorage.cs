namespace JabaBox.DataAccess.Repositories.FileSystemStorages.Abstractions;

public interface IFileSystemBaseDirectoryStorage
{
    void CreateBaseDirectory(Guid accountId);
    void CheckBaseDirectory(Guid accountId);
}