using JabaBox.Core.Domain.Entities;
using JabaBox.WebApi.Dto;
using JabaBox.WebApi.Mappers.Abstractions;

namespace JabaBox.WebApi.Mappers.Implementations;

public class BaseDirectoryMapper : IBaseDirectoryMapper
{
    public BaseDirectoryDto EntityToAccountInfoDto(BaseDirectory directory)
    {
        var dirNames = directory.Directories.Select(d => d.Name).ToList();
        return new BaseDirectoryDto
        {
            Id = directory.UserId,
            BytesOccupied = directory.BytesOccupied,
            Directories = dirNames,
        };
    }
}