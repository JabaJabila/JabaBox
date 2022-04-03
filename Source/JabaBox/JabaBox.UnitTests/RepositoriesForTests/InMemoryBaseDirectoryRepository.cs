using System;
using System.Linq;
using System.Threading.Tasks;
using JabaBox.Core.Domain.Entities;
using JabaBox.Core.Domain.Exceptions;
using JabaBox.Core.RepositoryAbstractions;
using JabaBox.UnitTests.DbContextsForTests;
using Microsoft.EntityFrameworkCore;

namespace JabaBox.UnitTests.RepositoriesForTests;

public class InMemoryBaseDirectoryRepository : IBaseDirectoryRepository
{
    private readonly JabaBoxDbTestContext _context;

    public InMemoryBaseDirectoryRepository(JabaBoxDbTestContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }
    
    public void CreateBaseDirectory(Guid accountId)
    {
        var baseDirectory = new BaseDirectory(accountId);
        _context.BaseDirectories.Add(baseDirectory);
        _context.SaveChanges();
    }

    public BaseDirectory GetBaseDirectoryById(Guid accountId)
    {
        var baseDirectory = _context.BaseDirectories.First(d => d.UserId == accountId);
        _context.Entry(baseDirectory).Collection(d => d.Directories).Load();
        if (baseDirectory is null)
            throw new DirectoryException($"Base directory not found for id \'{accountId}\'");
        
        return baseDirectory;
    }

    public BaseDirectory UpdateBaseDirectory(BaseDirectory directory)
    {
        ArgumentNullException.ThrowIfNull(directory);
        _context.BaseDirectories.Update(directory);
        _context.SaveChanges();
        return directory;
    }
}