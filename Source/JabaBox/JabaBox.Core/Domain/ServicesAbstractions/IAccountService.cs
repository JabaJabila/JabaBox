using JabaBox.Core.Domain.Entities;

namespace JabaBox.Core.Domain.ServicesAbstractions;

public interface IAccountService
{
    AccountInfo GetAccount(string login);
    AccountInfo GetAccount(Guid id);
    AccountInfo RegisterAccount(string login, string password, int gigabytesAvailable);
    AccountInfo ChangePassword(string login, string oldPassword, string newPassword);
    AccountInfo ChangeGigabytesPlan(string login, int newGigabytesAvailable);
}