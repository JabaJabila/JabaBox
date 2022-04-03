using JabaBox.Core.Domain.Exceptions;

namespace JabaBox.Core.Domain.Entities;

public class StorageDirectory
{
    private List<StorageFile> _files;

    public StorageDirectory(string name, BaseDirectory baseDirectory)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DirectoryException("Directory name can't be null");

        BaseDirectory = baseDirectory ?? throw new ArgumentNullException(nameof(baseDirectory));
        
        _files = new List<StorageFile>();
        Name = name;
    }

    private StorageDirectory()
    {
    }
    
    public Guid Id { get; set; }
    public BaseDirectory BaseDirectory { get; private init; }
    public string Name { get; set; }
    public IReadOnlyCollection<StorageFile> Files
    {
        get => _files;
        set => _files = value.ToList() ?? throw new ArgumentNullException(nameof(value));
    }
}