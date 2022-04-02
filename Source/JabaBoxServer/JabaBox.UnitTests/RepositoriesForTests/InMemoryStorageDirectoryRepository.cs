using System;
using System.Threading.Tasks;
using JabaBox.Core.Domain.Entities;
using JabaBox.Core.RepositoryAbstractions;
using JabaBox.UnitTests.DbContextsForTests;
using Microsoft.EntityFrameworkCore;

namespace JabaBox.UnitTests.RepositoriesForTests;

public class InMemoryStorageDirectoryRepository : IStorageDirectoryRepository
{
    private readonly JabaBoxDbTestContext _context;

    public InMemoryStorageDirectoryRepository(JabaBoxDbTestContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }
    
    public async Task<StorageDirectory?> FindDirectory(AccountInfo account, string name)
    {
        ArgumentNullException.ThrowIfNull(account);
        ArgumentNullException.ThrowIfNull(name);
        var directory = await _context.StorageDirectories
            .FirstOrDefaultAsync(d => d.Name == name && d.BaseDirectory.UserId == account.Id);

        if (directory is null)
            return directory;

        await _context.Entry(directory).Reference(d => d.BaseDirectory).LoadAsync();
        await _context.Entry(directory).Collection(d => d.Files).LoadAsync();

        return directory;
    }

    public async Task<StorageDirectory> UpdateStorageDirectory(StorageDirectory directory)
    {
        ArgumentNullException.ThrowIfNull(directory);

        _context.StorageDirectories.Update(directory);
        await _context.SaveChangesAsync();
        return directory;
    }

    public async Task<StorageDirectory> CreateDirectory(StorageDirectory directory)
    {
        ArgumentNullException.ThrowIfNull(directory);

        var temp = await _context.AddAsync(directory);
        directory = temp.Entity;
        await _context.SaveChangesAsync();
        return directory;
    }

    public async Task DeleteDirectory(StorageDirectory directory)
    {
        ArgumentNullException.ThrowIfNull(directory);

        _context.Remove(directory);
        await _context.SaveChangesAsync();
    }
}