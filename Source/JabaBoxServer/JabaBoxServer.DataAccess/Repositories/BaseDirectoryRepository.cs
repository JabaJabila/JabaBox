using JabaBox.Core.Domain.Entities;
using JabaBox.Core.Domain.Exceptions;
using JabaBox.Core.RepositoryAbstractions;
using JabaBoxServer.DataAccess.DataBaseContexts;
using JabaBoxServer.DataAccess.Repositories.FileSystemStorages.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace JabaBoxServer.DataAccess.Repositories;

public class BaseDirectoryRepository : IBaseDirectoryRepository
{
    private readonly JabaBoxDbContext _context;
    private readonly IFileSystemBaseDirectoryStorage _storage;

    public BaseDirectoryRepository(JabaBoxDbContext context, IFileSystemBaseDirectoryStorage storage)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _storage = storage ?? throw new ArgumentNullException(nameof(storage));
    }
    
    public async void CreateBaseDirectory(Guid accountId)
    {
        var baseDirectory = new BaseDirectory(accountId);
        _storage.CreateBaseDirectory(accountId);
        await _context.BaseDirectories.AddAsync(baseDirectory);
        await _context.SaveChangesAsync();
    }

    public async Task<BaseDirectory> GetBaseDirectoryById(Guid accountId)
    {
        var baseDirectory = await _context.BaseDirectories.FirstOrDefaultAsync(d => d.UserId == accountId);
        _storage.CheckBaseDirectory(accountId);
        if (baseDirectory is null)
            throw new DirectoryException($"Base directory not found for id \'{accountId}\'");
        
        return baseDirectory;
    }

    public async Task<BaseDirectory> UpdateBaseDirectory(BaseDirectory directory)
    {
        ArgumentNullException.ThrowIfNull(directory);
        _context.BaseDirectories.Update(directory);
        _storage.CheckBaseDirectory(directory.UserId);
        await _context.SaveChangesAsync();
        return directory;
    }
}