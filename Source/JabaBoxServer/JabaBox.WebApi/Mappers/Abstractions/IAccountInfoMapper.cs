using JabaBox.Core.Domain.Entities;
using JabaBox.WebApi.Dto;

namespace JabaBox.WebApi.Mappers.Abstractions;

public interface IAccountInfoMapper : IMapper
{
    AccountInfoDto EntityToAccountInfoDto(AccountInfo accountInfo);
}