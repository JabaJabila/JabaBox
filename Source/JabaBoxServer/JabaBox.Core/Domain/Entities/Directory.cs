using JabaBox.Core.Domain.Exceptions;

namespace JabaBox.Core.Domain.Entities;

public class Directory
{
    private List<File> _files;

    public Directory(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DirectoryException("Directory name can't be null");

        _files = new List<File>();
        Name = name;
    }

    private Directory()
    {
    }
    
    public Guid Id { get; set; }
    public string Name { get; set; }
    public IReadOnlyCollection<File> Files
    {
        get => _files;
        set => _files = value.ToList() ?? throw new ArgumentNullException(nameof(value));
    }
}