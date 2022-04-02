using JabaBox.Core.Domain.Entities;
using JabaBox.WebApi.Dto;
using JabaBox.WebApi.Mappers.Abstractions;

namespace JabaBox.WebApi.Mappers.Implementations;

public class StorageDirectoryMapper : IStorageDirectoryMapper
{
    public StorageDirectoryDto EntityToAccountInfoDto(StorageDirectory directory)
    {
        var fileNames = directory.Files.Select(f => f.Name).ToList();
        return new StorageDirectoryDto
        {
            Id = directory.Id,
            Name = directory.Name,
            Files = fileNames,
        };
    }
}