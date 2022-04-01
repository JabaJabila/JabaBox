using JabaBox.Core.Domain.Exceptions;

namespace JabaBox.Core.Domain.Entities;

public class StorageDirectory
{
    private List<StorageFile> _files;

    public StorageDirectory(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DirectoryException("Directory name can't be null");

        _files = new List<StorageFile>();
        Name = name;
    }

    private StorageDirectory()
    {
    }
    
    public Guid Id { get; set; }
    public string Name { get; set; }
    public IReadOnlyCollection<StorageFile> Files
    {
        get => _files;
        set => _files = value.ToList() ?? throw new ArgumentNullException(nameof(value));
    }
}