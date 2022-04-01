using JabaBox.Core.Domain.Entities;
using JabaBox.Core.Domain.Enums;
using JabaBox.Core.Domain.Exceptions;
using JabaBox.Core.Domain.ServicesAbstractions;
using JabaBox.Core.RepositoryAbstractions;

namespace JabaBox.Core.Services;

public class StorageService : IStorageService
{
    private readonly IBaseDirectoryRepository _baseDirectoryRepository;
    private readonly IStorageDirectoryRepository _storageDirectoryRepository;
    private readonly IStorageFileRepository _storageFileRepository;

    public StorageService(
        IBaseDirectoryRepository baseDirRepo,
        IStorageDirectoryRepository storeDirRepo,
        IStorageFileRepository storeFileRepo)
    {
        _baseDirectoryRepository = baseDirRepo ?? throw new ArgumentNullException(nameof(baseDirRepo));
        _storageDirectoryRepository = storeDirRepo ?? throw new ArgumentNullException(nameof(storeDirRepo));
        _storageFileRepository = storeFileRepo ?? throw new ArgumentNullException(nameof(storeFileRepo));
    }

    public StorageDirectory? FindDirectory(AccountInfo account, string name)
    {
        ArgumentNullException.ThrowIfNull(account);
        ArgumentNullException.ThrowIfNull(name);

        return _storageDirectoryRepository.FindDirectory(account, name).Result;
    }

    public StorageFile? FindFile(AccountInfo account, StorageDirectory directory, string name)
    {
        ArgumentNullException.ThrowIfNull(account);
        ArgumentNullException.ThrowIfNull(directory);
        ArgumentNullException.ThrowIfNull(name);

        return _storageFileRepository.FindFile(account, directory, name).Result;
    }

    public StorageDirectory CreateDirectory(AccountInfo account, string name)
    {
        ArgumentNullException.ThrowIfNull(account);
        if (string.IsNullOrWhiteSpace(name))
            throw new DirectoryException("Name of directory can't be null or empty");
        
        BaseDirectory baseDirectory = _baseDirectoryRepository.GetBaseDirectoryById(account.Id).Result;

        if (FindDirectory(account, name) is not null)
            throw new DirectoryException($"Directory with name \'{name}\' already exist");

        var directory = new StorageDirectory(name, baseDirectory);
        baseDirectory.Directories.ToList().Add(directory);
        _baseDirectoryRepository.UpdateBaseDirectory(baseDirectory);
        return _storageDirectoryRepository.CreateDirectory(directory).Result;
    }

    public StorageDirectory RenameDirectory(AccountInfo account, StorageDirectory directory, string newName)
    {
        ArgumentNullException.ThrowIfNull(account);
        ArgumentNullException.ThrowIfNull(directory);
        ArgumentNullException.ThrowIfNull(newName);

        CheckIfDirectoryExists(account, directory);

        var baseDir = _baseDirectoryRepository.GetBaseDirectoryById(account.Id).Result;
        if (baseDir.Directories.Any(d => d.Name == newName))
            throw new DirectoryException($"Directory with name \'{newName}\' already exists");
        
        directory.Name = newName;
        return _storageDirectoryRepository.UpdateStorageDirectory(directory).Result;
    }

    public void DeleteDirectory(AccountInfo account, StorageDirectory directory)
    {
        ArgumentNullException.ThrowIfNull(account);
        ArgumentNullException.ThrowIfNull(directory);

        BaseDirectory baseDirectory = _baseDirectoryRepository.GetBaseDirectoryById(account.Id).Result;
        if (!baseDirectory.Directories.Any(d => d.Id == directory.Id && d.Name == directory.Name))
            throw new DirectoryException($"Directory with name \'{directory.Name}\' doesn't exist");
        
        CheckIfDirectoryExists(account, directory);

        baseDirectory.Directories.ToList().Remove(directory);
        _baseDirectoryRepository.UpdateBaseDirectory(baseDirectory);
        _storageDirectoryRepository.DeleteDirectory(directory);
    }

    public StorageFile AddFile(AccountInfo account, StorageDirectory directory, FileState state, string name, byte[] data)
    {
        ArgumentNullException.ThrowIfNull(account);
        ArgumentNullException.ThrowIfNull(directory);
        if (string.IsNullOrWhiteSpace(name))
            throw new DirectoryException("Name of file can't be null or empty");

        var baseDir = _baseDirectoryRepository.GetBaseDirectoryById(account.Id).Result;
        CheckIfDirectoryExists(account, directory);

        if (directory.Files.Any(f => f.Name == name))
            throw new FileException($"File with name \'{name}\' already exists");

        if (BytesAvailable(account) < data.Length)
            throw new FileException($"Not enough space in storage for account \'{account.Login}\'");
        
        var file = _storageFileRepository
            .AddFile(new StorageFile(name, state, data.Length, directory), data, directory).Result;
        directory.Files.ToList().Add(file);
        baseDir.BytesOccupied += data.Length;
        _baseDirectoryRepository.UpdateBaseDirectory(baseDir);
        _storageDirectoryRepository.UpdateStorageDirectory(directory);
        return file;
    }

    public StorageFile RenameFile(AccountInfo account, StorageDirectory directory, StorageFile file, string newName)
    {
        ArgumentNullException.ThrowIfNull(account);
        ArgumentNullException.ThrowIfNull(directory);
        ArgumentNullException.ThrowIfNull(file);
        if (string.IsNullOrWhiteSpace(newName))
            throw new DirectoryException("Name of file can't be null or empty");

        CheckIfDirectoryExists(account, directory);
        CheckIfFileExists(account, directory, file);

        if (directory.Files.Any(f => f.Name == newName))
            throw new FileException($"File with name \'{newName}\' already exists");
        
        file.Name = newName;
        return _storageFileRepository.UpdateFile(file).Result;
    }

    public void DeleteFile(AccountInfo account, StorageDirectory directory, StorageFile file)
    {
        ArgumentNullException.ThrowIfNull(account);
        ArgumentNullException.ThrowIfNull(directory);
        ArgumentNullException.ThrowIfNull(file);

        CheckIfDirectoryExists(account, directory);
        CheckIfFileExists(account, directory, file);

        directory.Files.ToList().Remove(file);
        _storageDirectoryRepository.UpdateStorageDirectory(directory);
        _storageFileRepository.DeleteFile(file);
    }

    public long BytesAvailable(AccountInfo account)
    {
        ArgumentNullException.ThrowIfNull(account);

        var baseDir = _baseDirectoryRepository.GetBaseDirectoryById(account.Id).Result;
        return TranslateGigabytesToBytes(account.GigabytesAvailable) - baseDir.BytesOccupied;
    }

    private static long TranslateGigabytesToBytes(int gigabytes)
    {
        return gigabytes * 1024 * 1024 * 1024;
    }
    
    private void CheckIfDirectoryExists(AccountInfo account, StorageDirectory directory)
    {
        StorageDirectory? foundDir = FindDirectory(account, directory.Name);
        if (foundDir is null || foundDir.Id != directory.Id)
            throw new DirectoryException($"Directory with name \'{directory.Name}\' doesn't exist");
    }
    
    private void CheckIfFileExists(AccountInfo account, StorageDirectory directory, StorageFile file)
    {
        StorageFile? fileFound = FindFile(account, directory, file.Name);
        if (fileFound is null || fileFound.Id != file.Id)
            throw new DirectoryException($"File with name \'{file.Name}\' doesn't exist");
    }
}