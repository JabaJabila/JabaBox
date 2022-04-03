using System;
using System.Linq;
using JabaBox.Core.Domain.Entities;
using JabaBox.Core.RepositoryAbstractions;
using JabaBox.UnitTests.DbContextsForTests;

namespace JabaBox.UnitTests.RepositoriesForTests;

public class InMemoryStorageDirectoryRepository : IStorageDirectoryRepository
{
    private readonly JabaBoxDbTestContext _context;

    public InMemoryStorageDirectoryRepository(JabaBoxDbTestContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
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
        _context.SaveChanges();
        return directory;
    }

    public StorageDirectory CreateDirectory(StorageDirectory directory)
    {
        ArgumentNullException.ThrowIfNull(directory);

        var temp = _context.Add(directory);
        directory = temp.Entity;
        _context.SaveChanges();
        return directory;
    }

    public void DeleteDirectory(StorageDirectory directory)
    {
        ArgumentNullException.ThrowIfNull(directory);

        _context.Remove(directory);
        _context.SaveChanges();
    }
}