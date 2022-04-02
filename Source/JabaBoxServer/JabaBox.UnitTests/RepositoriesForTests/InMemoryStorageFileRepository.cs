using System;
using System.Threading.Tasks;
using JabaBox.Core.Domain.Entities;
using JabaBox.Core.RepositoryAbstractions;
using JabaBox.UnitTests.DbContextsForTests;
using Microsoft.EntityFrameworkCore;

namespace JabaBox.UnitTests.RepositoriesForTests;

public class InMemoryStorageFileRepository : IStorageFileRepository
{
    private readonly JabaBoxDbTestContext _context;

    public InMemoryStorageFileRepository(JabaBoxDbTestContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }
    
    public async Task<StorageFile?> FindFile(AccountInfo account, StorageDirectory directory, string name)
    {
        ArgumentNullException.ThrowIfNull(account);
        ArgumentNullException.ThrowIfNull(directory);
        ArgumentNullException.ThrowIfNull(name);

        var file = await _context.StorageFiles
            .FirstOrDefaultAsync(f => f.Directory.Id == directory.Id && f.Name == name);

        if (file is null)
            return file;

        await _context.Entry(file).Reference(f => f.Directory).LoadAsync();
        await _context.Entry(file.Directory).Reference(d => d.BaseDirectory).LoadAsync();
        return file;
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

    public async Task DeleteFile(StorageFile file)
    {
        ArgumentNullException.ThrowIfNull(file);

        _context.StorageFiles.Remove(file);
        await _context.SaveChangesAsync();
    }
}