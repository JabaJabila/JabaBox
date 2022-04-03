using JabaBox.Core.Domain.Entities;
using JabaBox.Core.Domain.Enums;
using JabaBox.WebApi.Dto;
using JabaBox.WebApi.Mappers.Abstractions;

namespace JabaBox.WebApi.Mappers.Implementations;

public class StorageFileMapper : IStorageFileMapper
{
    public StorageFileDto EntityToDto(StorageFile file)
    {
        ArgumentNullException.ThrowIfNull(file);
        return new StorageFileDto
        {
            Id = file.Id,
            Name = file.Name,
            Size = file.ByteSize,
            State = file.State == FileState.Normal ? "normal" : "compressed",
        };
    }
}