using JabaBox.Core.Domain.Entities;
using JabaBox.Core.Domain.Exceptions;
using JabaBox.Core.RepositoryAbstractions;
using JabaBox.DataAccess.DataBaseContexts;
using JabaBox.DataAccess.Repositories.FileSystemStorages.Abstractions;

namespace JabaBox.DataAccess.Repositories;

public class BaseDirectoryRepository : IBaseDirectoryRepository
{
    private readonly JabaBoxDbContext _context;
    private readonly IFileSystemBaseDirectoryStorage _storage;

    public BaseDirectoryRepository(JabaBoxDbContext context, IFileSystemBaseDirectoryStorage storage)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _storage = storage ?? throw new ArgumentNullException(nameof(storage));
    }
    
    public void CreateBaseDirectory(Guid accountId)
    {
        var baseDirectory = new BaseDirectory(accountId);
        _storage.CreateBaseDirectory(accountId);
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
        _storage.CheckBaseDirectory(directory.UserId);
        _context.SaveChanges();
        return directory;
    }
}