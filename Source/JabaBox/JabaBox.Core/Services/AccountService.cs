using JabaBox.Core.Domain.Entities;
using JabaBox.Core.Domain.Exceptions;
using JabaBox.Core.Domain.ServicesAbstractions;
using JabaBox.Core.RepositoryAbstractions;

namespace JabaBox.Core.Services;

public class AccountService : IAccountService
{
    private readonly IAccountInfoRepository _accountInfoRepository;
    private readonly IBaseDirectoryRepository _baseDirectoryRepository;
    
    public AccountService(IAccountInfoRepository accRepo, IBaseDirectoryRepository basedirRepo)
    {
        _accountInfoRepository = accRepo ?? throw new ArgumentNullException(nameof(accRepo));
        _baseDirectoryRepository = basedirRepo ?? throw new ArgumentNullException(nameof(basedirRepo));
    }

    public AccountInfo GetAccount(string login)
    {
        AccountInfo? account = _accountInfoRepository.FindAccountByLogin(login);
        if (account is null)
            throw new AccountInfoException($"Account with login \'{login}\' not found");

        return account;
    }

    public AccountInfo GetAccount(Guid id)
    {
        AccountInfo? account = _accountInfoRepository.FindAccountById(id);
        if (account is null)
            throw new AccountInfoException($"Account with id \'{id}\' not found");

        return account;
    }

    public AccountInfo RegisterAccount(string login, string password, int gigabytesAvailable)
    {
        if (string.IsNullOrWhiteSpace(login))
            throw new AccountInfoException("Impossible to set null or empty string as a login");
        
        if (string.IsNullOrWhiteSpace(password))
            throw new AccountInfoException("Impossible to set null or empty string as a password");

        if (gigabytesAvailable <= 0)
            throw new AccountInfoException("Impossible to set limit for <= 0 gigabytes");
        
        if (_accountInfoRepository.CheckIfLoginExists(login))
            throw new AccountInfoException($"Account with login \'{login}\' already exists");

        AccountInfo account = _accountInfoRepository
            .SaveAccountInfo(new AccountInfo(login, password, gigabytesAvailable));

        _baseDirectoryRepository.CreateBaseDirectory(account.Id);
        return account;
    }

    public AccountInfo ChangePassword(string login, string oldPassword, string newPassword)
    {
        if (string.IsNullOrWhiteSpace(newPassword))
            throw new AccountInfoException("Impossible to set null or empty string as password");

        AccountInfo account = GetAccount(login);

        if (account.Password != oldPassword)
            throw new AccountInfoException($"Wrong password for \'{login}\' account");

        if (newPassword== oldPassword)
            throw new AccountInfoException($"Old and new passwords for \'{login}\' account are same");
        
        account.Password = newPassword;
        _accountInfoRepository.UpdateAccountInfo(account);
        return account;
    }

    public AccountInfo ChangeGigabytesPlan(string login, int newGigabytesAvailable)
    {
        AccountInfo account = GetAccount(login);

        if (newGigabytesAvailable <= 0)
            throw new AccountInfoException("Impossible to set limit for <= 0 gigabytes");

        if ((long) newGigabytesAvailable * 1024 * 1024 * 1024
            < _baseDirectoryRepository.GetBaseDirectoryById(account.Id).BytesOccupied)
            throw new AccountInfoException("Not enough space left");

        account.GigabytesAvailable = newGigabytesAvailable;
        return _accountInfoRepository.UpdateAccountInfo(account);
    }
}