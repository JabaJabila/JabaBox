using JabaBox.Core.Domain.Entities;

namespace JabaBox.Core.RepositoryAbstractions;

public interface IBaseDirectoryRepository
{
    Task CreateBaseDirectory(Guid accountId);
    Task<BaseDirectory> GetBaseDirectoryById(Guid accountId);
    Task<BaseDirectory> UpdateBaseDirectory(BaseDirectory directory);
}