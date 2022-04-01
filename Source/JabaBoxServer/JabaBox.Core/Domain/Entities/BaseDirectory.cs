using JabaBox.Core.Domain.Exceptions;

namespace JabaBox.Core.Domain.Entities;

public class BaseDirectory
{
    private List<StorageDirectory> _directories;
    
    public BaseDirectory(Guid id)
    {
        if (id == Guid.Empty)
            throw new DirectoryException("Impossible to create base directory for empty guid id");

        _directories = new List<StorageDirectory>();
        BytesOccupied = 0;
    }

    private BaseDirectory()
    {
    }
    
    public Guid UserId { get; private init; }
    public long BytesOccupied { get; set; }
    public IReadOnlyCollection<StorageDirectory> Directories
    { 
        get => _directories;
        set => _directories = value.ToList() ?? throw new ArgumentNullException(nameof(value));
    }
}