using JabaBox.Core.Domain.Entities;
using JabaBox.Core.RepositoryAbstractions;
using JabaBox.DataAccess.DataBaseContexts;

namespace JabaBox.DataAccess.Repositories;

public class AccountInfoRepository : IAccountInfoRepository
{
    private readonly JabaBoxDbContext _context;

    public AccountInfoRepository(JabaBoxDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }
    
    public bool CheckIfLoginExists(string login)
    {
        ArgumentNullException.ThrowIfNull(login);
        return _context.AccountInfos.Any(a => a.Login == login);
    }

    public AccountInfo SaveAccountInfo(AccountInfo account)
    {
        ArgumentNullException.ThrowIfNull(account);
        
        var temp = _context.AccountInfos.Add(account);
        account = temp.Entity;
        _context.SaveChanges();
        return account;
    }

    public AccountInfo? FindAccountByLogin(string login)
    {
        ArgumentNullException.ThrowIfNull(login);
        
        return _context.AccountInfos.FirstOrDefault(a => a.Login == login);
    }

    public AccountInfo? FindAccountById(Guid id)
    {
        return _context.AccountInfos.FirstOrDefault(a => a.Id == id);
    }

    public AccountInfo UpdateAccountInfo(AccountInfo account)
    {
        ArgumentNullException.ThrowIfNull(account);
        
        _context.AccountInfos.Update(account);
        _context.SaveChanges();
        return account;
    }
}