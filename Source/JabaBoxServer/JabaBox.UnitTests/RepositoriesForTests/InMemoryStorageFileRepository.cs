using System;
using System.Threading.Tasks;
using JabaBox.Core.Domain.Entities;
using JabaBox.Core.RepositoryAbstractions;
using JabaBoxServer.DataAccess.DataBaseContexts;
using Microsoft.EntityFrameworkCore;

namespace JabaBox.UnitTests.RepositoriesForTests;

public class InMemoryStorageFileRepository : IStorageFileRepository
{
    private readonly JabaBoxDbContext _context;

    public InMemoryStorageFileRepository(JabaBoxDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }
    
    public async Task<StorageFile?> FindFile(AccountInfo account, StorageDirectory directory, string name)
    {
        ArgumentNullException.ThrowIfNull(account);
        ArgumentNullException.ThrowIfNull(directory);
        ArgumentNullException.ThrowIfNull(name);

        return await _context.StorageFiles.FirstOrDefaultAsync(f => f.Name == name);
    }

    public async Task<StorageFile> AddFile(StorageFile storageFile, byte[] data, StorageDirectory directory)
    {
        ArgumentNullException.ThrowIfNull(storageFile);
        ArgumentNullException.ThrowIfNull(data);
        ArgumentNullException.ThrowIfNull(directory);

        var temp = await _context.StorageFiles.AddAsync(storageFile);
        storageFile = temp.Entity;
        await _context.SaveChangesAsync();
        return storageFile;
    }

    public async Task<StorageFile> UpdateFile(StorageFile file)
    {
        ArgumentNullException.ThrowIfNull(file);

        _context.StorageFiles.Update(file);
        await _context.SaveChangesAsync();
        return file;
    }

    public async void DeleteFile(StorageFile file)
    {
        ArgumentNullException.ThrowIfNull(file);

        _context.StorageFiles.Remove(file);
        await _context.SaveChangesAsync();
    }
}