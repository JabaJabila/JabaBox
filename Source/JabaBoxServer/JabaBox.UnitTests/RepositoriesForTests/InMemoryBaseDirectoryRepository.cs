using System;
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
    
    public async void CreateBaseDirectory(Guid accountId)
    {
        var baseDirectory = new BaseDirectory(accountId);
        await _context.BaseDirectories.AddAsync(baseDirectory);
        await _context.SaveChangesAsync();
    }

    public async Task<BaseDirectory> GetBaseDirectoryById(Guid accountId)
    {
        var baseDirectory = await _context.BaseDirectories.FirstOrDefaultAsync(d => d.UserId == accountId);
        if (baseDirectory is null)
            throw new DirectoryException($"Base directory not found for id \'{accountId}\'");
        
        return baseDirectory;
    }

    public async Task<BaseDirectory> UpdateBaseDirectory(BaseDirectory directory)
    {
        ArgumentNullException.ThrowIfNull(directory);
        _context.BaseDirectories.Update(directory);
        await _context.SaveChangesAsync();
        return directory;
    }
}