using JabaBox.Core.Domain.Entities;
using JabaBox.WebApi.Dto;
using JabaBox.WebApi.Mappers.Abstractions;

namespace JabaBox.WebApi.Mappers.Implementations;

public class StorageDirectoryMapper : IStorageDirectoryMapper
{
    private readonly IStorageFileMapper _storageFileMapper;
    
    public StorageDirectoryMapper(IStorageFileMapper storageFileMapper)
    {
        _storageFileMapper = storageFileMapper ?? throw new ArgumentNullException(nameof(storageFileMapper));
    }

    public StorageDirectoryDto EntityToDto(StorageDirectory directory)
    {
        ArgumentNullException.ThrowIfNull(directory);
        var fileNames = directory.Files.Select(f => _storageFileMapper.EntityToDto(f)).ToList();
        return new StorageDirectoryDto
        {
            Id = directory.Id,
            Name = directory.Name,
            Files = fileNames,
        };
    }
}