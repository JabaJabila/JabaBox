using JabaBox.Core.Domain.Enums;
using JabaBox.Core.Domain.Exceptions;

namespace JabaBox.Core.Domain.Entities;

public class StorageFile
{
    public StorageFile(string name, FileState state, long size)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new FileException("File name can't be empty");

        if (size <= 0)
            throw new FileException("File size can't be <= 0 bytes");

        Name = name;
        State = state;
        ByteSize = size;
    }

    private StorageFile()
    {
    }
    
    public Guid Id { get; private init; }
    public string Name { get; set; }
    public FileState State { get; set; }
    public long ByteSize { get; private init; }
}