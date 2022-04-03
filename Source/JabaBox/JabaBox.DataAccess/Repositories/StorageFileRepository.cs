using JabaBox.Core.Domain.Entities;
using JabaBox.Core.RepositoryAbstractions;
using JabaBox.DataAccess.DataBaseContexts;
using JabaBox.DataAccess.Repositories.FileSystemStorages.Abstractions;

namespace JabaBox.DataAccess.Repositories;

public class StorageFileRepository : IStorageFileRepository
{
    private readonly JabaBoxDbContext _context;
    private readonly IFileSystemStorageFileStorage _storage;

    public StorageFileRepository(JabaBoxDbContext context, IFileSystemStorageFileStorage storage)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _storage = storage ?? throw new ArgumentNullException(nameof(storage));
    }
    
    public StorageFile? FindFile(AccountInfo account, StorageDirectory directory, string name)
    {
        ArgumentNullException.ThrowIfNull(account);
        ArgumentNullException.ThrowIfNull(directory);
        ArgumentNullException.ThrowIfNull(name);

        var file = _context.StorageFiles
            .FirstOrDefault(f => f.Directory.Id == directory.Id && f.Name == name);

        if (file is null)
            return file;

        _context.Entry(file).Reference(f => f.Directory).Load(); 
        _context.Entry(file.Directory).Reference(d => d.BaseDirectory).Load();
        return file;
    }

    public StorageFile AddFile(StorageFile storageFile, byte[] data, StorageDirectory directory)
    {
        ArgumentNullException.ThrowIfNull(storageFile);
        ArgumentNullException.ThrowIfNull(data);
        ArgumentNullException.ThrowIfNull(directory);

        var temp = _context.StorageFiles.Add(storageFile);
        storageFile = temp.Entity;
        _storage.CreateStorageFile(directory.BaseDirectory.UserId, directory.Id, storageFile.Id, data);
        _context.SaveChanges();
        return storageFile;
    }

    public StorageFile UpdateFile(StorageFile file)
    {
        ArgumentNullException.ThrowIfNull(file);

        _context.StorageFiles.Update(file);
        _storage.CheckStorageFile(file.Directory.BaseDirectory.UserId, file.Directory.Id, file.Id);
        _context.SaveChanges();
        return file;
    }

    public void DeleteFile(StorageFile file)
    {
        ArgumentNullException.ThrowIfNull(file);

        _context.StorageFiles.Remove(file);
        _storage.DeleteStorageFile(file.Directory.BaseDirectory.UserId, file.Directory.Id, file.Id);
        _context.SaveChanges();
    }

    public byte[] GetFileData(AccountInfo account, StorageDirectory directory, StorageFile file)
    {
        ArgumentNullException.ThrowIfNull(account);
        ArgumentNullException.ThrowIfNull(directory);
        ArgumentNullException.ThrowIfNull(file);

        return _storage.GetFileData(account.Id, directory.Id, file.Id);
    }
}