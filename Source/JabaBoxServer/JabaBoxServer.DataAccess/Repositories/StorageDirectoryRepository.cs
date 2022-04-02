using JabaBox.Core.Domain.Entities;
using JabaBox.Core.RepositoryAbstractions;
using JabaBoxServer.DataAccess.DataBaseContexts;
using JabaBoxServer.DataAccess.Repositories.FileSystemStorages.Abstractions;

namespace JabaBoxServer.DataAccess.Repositories;

public class StorageDirectoryRepository : IStorageDirectoryRepository
{
    private readonly JabaBoxDbContext _context;
    private readonly IFileSystemStorageDirectoryStorage _storage;

    public StorageDirectoryRepository(JabaBoxDbContext context, IFileSystemStorageDirectoryStorage storage)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _storage = storage ?? throw new ArgumentNullException(nameof(storage));
    }
    
    public StorageDirectory? FindDirectory(AccountInfo account, string name)
    {
        ArgumentNullException.ThrowIfNull(account);
        ArgumentNullException.ThrowIfNull(name);
        var directory = _context.StorageDirectories
            .FirstOrDefault(d => d.Name == name && d.BaseDirectory.UserId == account.Id);

        if (directory is null)
            return directory;

        _context.Entry(directory).Reference(d => d.BaseDirectory).Load();
        _context.Entry(directory).Collection(d => d.Files).Load();

        return directory;
    }

    public StorageDirectory UpdateStorageDirectory(StorageDirectory directory)
    {
        ArgumentNullException.ThrowIfNull(directory);

        _context.StorageDirectories.Update(directory);
        _storage.CheckStorageDirectory(directory.BaseDirectory.UserId, directory.Id);
        _context.SaveChanges();
        return directory;
    }

    public StorageDirectory CreateDirectory(StorageDirectory directory)
    {
        ArgumentNullException.ThrowIfNull(directory);

        var temp = _context.Add(directory);
        directory = temp.Entity;
        _storage.CreateStorageDirectory(directory.BaseDirectory.UserId, directory.Id);
        _context.SaveChanges();
        return directory;
    }

    public void DeleteDirectory(StorageDirectory directory)
    {
        ArgumentNullException.ThrowIfNull(directory);

        _context.Remove(directory);
        _storage.DeleteStorageDirectory(directory.BaseDirectory.UserId, directory.Id);
        _context.SaveChanges();
    }
}