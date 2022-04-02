using System;
using System.Linq;
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
        _context.SaveChanges();
        return storageFile;
    }

    public StorageFile UpdateFile(StorageFile file)
    {
        ArgumentNullException.ThrowIfNull(file);

        _context.StorageFiles.Update(file);
        _context.SaveChanges();
        return file;
    }

    public void DeleteFile(StorageFile file)
    {
        ArgumentNullException.ThrowIfNull(file);

        _context.StorageFiles.Remove(file);
        _context.SaveChanges();
    }
}