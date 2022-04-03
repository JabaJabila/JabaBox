using JabaBox.Core.Domain.Entities;
using JabaBox.Core.RepositoryAbstractions;
using JabaBox.WebApi.Dto;
using JabaBox.WebApi.Mappers.Abstractions;

namespace JabaBox.WebApi.Mappers.Implementations;

public class AccountInfoMapper : IAccountInfoMapper
{
    public AccountInfoDto EntityToDto(AccountInfo accountInfo)
    {
        ArgumentNullException.ThrowIfNull(accountInfo);
        return new AccountInfoDto { 
            Id = accountInfo.Id,
            Login = accountInfo.Login,
            Password = accountInfo.Password,
            GigabytesAvailable = accountInfo.GigabytesAvailable };
    }
}