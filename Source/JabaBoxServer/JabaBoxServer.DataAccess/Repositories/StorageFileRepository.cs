using JabaBox.Core.Domain.Entities;
using JabaBox.Core.RepositoryAbstractions;
using JabaBoxServer.DataAccess.DataBaseContexts;
using JabaBoxServer.DataAccess.Repositories.FileSystemStorages.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace JabaBoxServer.DataAccess.Repositories;

public class StorageFileRepository : IStorageFileRepository
{
    private readonly JabaBoxDbContext _context;
    private readonly IFileSystemStorageFileStorage _storage;

    public StorageFileRepository(JabaBoxDbContext context, IFileSystemStorageFileStorage storage)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _storage = storage ?? throw new ArgumentNullException(nameof(storage));
    }
    
    public async Task<StorageFile?> FindFile(AccountInfo account, StorageDirectory directory, string name)
    {
        ArgumentNullException.ThrowIfNull(account);
        ArgumentNullException.ThrowIfNull(directory);
        ArgumentNullException.ThrowIfNull(name);

        return await _context.StorageFiles.FirstOrDefaultAsync(f => f.Directory.Id == directory.Id && f.Name == name);
    }

    public async Task<StorageFile> AddFile(StorageFile storageFile, byte[] data, StorageDirectory directory)
    {
        ArgumentNullException.ThrowIfNull(storageFile);
        ArgumentNullException.ThrowIfNull(data);
        ArgumentNullException.ThrowIfNull(directory);

        var temp = await _context.StorageFiles.AddAsync(storageFile);
        storageFile = temp.Entity;
        _storage.CreateStorageFile(directory.BaseDirectory.UserId, directory.Id, storageFile.Id, data);
        await _context.SaveChangesAsync();
        return storageFile;
    }

    public async Task<StorageFile> UpdateFile(StorageFile file)
    {
        ArgumentNullException.ThrowIfNull(file);

        _context.StorageFiles.Update(file);
        _storage.CheckStorageFile(file.Directory.BaseDirectory.UserId, file.Directory.Id, file.Id);
        await _context.SaveChangesAsync();
        return file;
    }

    public async void DeleteFile(StorageFile file)
    {
        ArgumentNullException.ThrowIfNull(file);

        _context.StorageFiles.Remove(file);
        _storage.DeleteStorageFile(file.Directory.BaseDirectory.UserId, file.Directory.Id, file.Id);
        await _context.SaveChangesAsync();
    }
}