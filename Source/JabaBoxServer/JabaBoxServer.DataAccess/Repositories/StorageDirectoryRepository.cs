using JabaBox.Core.Domain.Entities;
using JabaBox.Core.RepositoryAbstractions;
using JabaBoxServer.DataAccess.DataBaseContexts;
using JabaBoxServer.DataAccess.Repositories.FileSystemStorages.Abstractions;
using Microsoft.EntityFrameworkCore;

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
    
    public async Task<StorageDirectory?> FindDirectory(AccountInfo account, string name)
    {
        ArgumentNullException.ThrowIfNull(account);
        ArgumentNullException.ThrowIfNull(name);
        
        return await _context.StorageDirectories
            .FirstOrDefaultAsync(d => d.Name == name && d.BaseDirectory.UserId == account.Id);
    }

    public async Task<StorageDirectory> UpdateStorageDirectory(StorageDirectory directory)
    {
        ArgumentNullException.ThrowIfNull(directory);

        _context.StorageDirectories.Update(directory);
        _storage.CheckStorageDirectory(directory.BaseDirectory.UserId, directory.Id);
        await _context.SaveChangesAsync();
        return directory;
    }

    public async Task<StorageDirectory> CreateDirectory(StorageDirectory directory)
    {
        ArgumentNullException.ThrowIfNull(directory);

        var temp = await _context.AddAsync(directory);
        directory = temp.Entity;
        _storage.CreateStorageDirectory(directory.BaseDirectory.UserId, directory.Id);
        await _context.SaveChangesAsync();
        return directory;
    }

    public async void DeleteDirectory(StorageDirectory directory)
    {
        ArgumentNullException.ThrowIfNull(directory);

        _context.Remove(directory);
        _storage.DeleteStorageDirectory(directory.BaseDirectory.UserId, directory.Id);
        await _context.SaveChangesAsync();
    }
}