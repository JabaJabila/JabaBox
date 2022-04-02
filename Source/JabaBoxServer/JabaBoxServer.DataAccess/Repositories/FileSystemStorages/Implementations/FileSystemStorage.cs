using JabaBox.Core.Domain.Exceptions;
using JabaBoxServer.DataAccess.Repositories.FileSystemStorages.Abstractions;

namespace JabaBoxServer.DataAccess.Repositories.FileSystemStorages.Implementations;

public class FileSystemStorage : IFileSystemStorage
{
    private readonly string _storageDir;

    public FileSystemStorage(string pathToStorageDir)
    {
        ArgumentNullException.ThrowIfNull(pathToStorageDir);
        Directory.CreateDirectory(pathToStorageDir);
        _storageDir = pathToStorageDir;
    }
    
    public void CreateBaseDirectory(Guid accountId)
    {
        Directory.CreateDirectory(Path.Combine(_storageDir, accountId.ToString()));
    }

    public void CheckBaseDirectory(Guid accountId)
    {
        if (!Directory.Exists(Path.Combine(_storageDir, accountId.ToString())))
            throw new DirectoryException($"Base directory for account with id {accountId} doesn't exist");
    }

    public void CreateStorageDirectory(Guid baseDirectoryUserId, Guid directoryId)
    {
        CheckBaseDirectory(baseDirectoryUserId);
        Directory.CreateDirectory(Path.Combine(_storageDir, baseDirectoryUserId.ToString(), directoryId.ToString()));
    }

    public void DeleteStorageDirectory(Guid baseDirectoryUserId, Guid directoryId)
    {
        var di = new DirectoryInfo(Path.Combine(_storageDir, baseDirectoryUserId.ToString(), directoryId.ToString()));
        FileInfo[] files = di.GetFiles();
        foreach (var file in files)
            file.Delete();
        
        Directory.Delete(Path.Combine(_storageDir, baseDirectoryUserId.ToString(), directoryId.ToString()));
    }

    public void CheckStorageDirectory(Guid baseDirectoryUserId, Guid directoryId)
    {
        if (!Directory.Exists(Path.Combine(_storageDir, baseDirectoryUserId.ToString(), directoryId.ToString())))
            throw new DirectoryException(
                $"Storage directory with id {directoryId} for user with id {baseDirectoryUserId} doesn't exist");
    }


    public void CreateStorageFile(Guid baseDirectoryUserId, Guid directoryId, Guid storageFileId, byte[] data)
    {
        CheckStorageDirectory(baseDirectoryUserId, directoryId);
        string pathToFile =
            Path.Combine(Path.Combine(_storageDir, baseDirectoryUserId.ToString(), directoryId.ToString()));

        File.WriteAllBytes(pathToFile, data);
    }

    public void CheckStorageFile(Guid baseDirectoryUserId, Guid directoryId, Guid fileId)
    {
        if (!File.Exists(Path.Combine(_storageDir, baseDirectoryUserId.ToString(), directoryId.ToString())))
            throw new FileException($"File with id {fileId} doesn't exist in directory with id {directoryId}" +
                                    $" for user with id {baseDirectoryUserId}");
    }

    public void DeleteStorageFile(Guid baseDirectoryUserId, Guid directoryId, Guid fileId)
    {
        var file = new FileInfo(Path.Combine(_storageDir, baseDirectoryUserId.ToString(), directoryId.ToString()));
        file.Delete();
    }
}