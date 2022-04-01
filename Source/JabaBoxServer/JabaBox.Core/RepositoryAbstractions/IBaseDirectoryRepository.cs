using JabaBox.Core.Domain.Entities;

namespace JabaBox.Core.RepositoryAbstractions;

public interface IBaseDirectoryRepository
{
    void CreateBaseDirectory(Guid accountId);
    BaseDirectory GetBaseDirectoryById(Guid accountId);
    BaseDirectory UpdateBaseDirectory(BaseDirectory directory);
}