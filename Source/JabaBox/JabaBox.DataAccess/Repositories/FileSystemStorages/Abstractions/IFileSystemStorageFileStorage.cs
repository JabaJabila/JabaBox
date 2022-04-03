namespace JabaBox.DataAccess.Repositories.FileSystemStorages.Abstractions;

public interface IFileSystemStorageFileStorage
{
    void CreateStorageFile(Guid baseDirectoryUserId, Guid directoryId, Guid storageFileId, byte[] data);
    void CheckStorageFile(Guid baseDirectoryUserId, Guid directoryId, Guid fileId);
    void DeleteStorageFile(Guid baseDirectoryUserId, Guid directoryId, Guid fileId);
    byte[] GetFileData(Guid accountId, Guid directoryId, Guid fileId);
}