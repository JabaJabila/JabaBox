using JabaBox.Core.Domain.Entities;
using JabaBox.Core.RepositoryAbstractions;
using JabaBoxServer.DataAccess.DataBaseContexts;
using Microsoft.EntityFrameworkCore;

namespace JabaBoxServer.DataAccess.Repositories;

public class AccountInfoRepository : IAccountInfoRepository
{
    private readonly JabaBoxDbContext _context;

    public AccountInfoRepository(JabaBoxDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }
    
    public async Task<bool> CheckIfLoginExists(string login)
    {
        ArgumentNullException.ThrowIfNull(login);
        return await _context.AccountInfos.AnyAsync(a => a.Login == login);
    }

    public async Task<AccountInfo> SaveAccountInfo(AccountInfo account)
    {
        ArgumentNullException.ThrowIfNull(account);
        
        var temp = await _context.AccountInfos.AddAsync(account);
        account = temp.Entity;
        await _context.SaveChangesAsync();
        return account;
    }

    public async Task<AccountInfo?> FindAccountByLogin(string login)
    {
        ArgumentNullException.ThrowIfNull(login);
        
        return await _context.AccountInfos.FirstOrDefaultAsync(a => a.Login == login);
    }

    public async Task<AccountInfo?> FindAccountById(Guid id)
    {
        return await _context.AccountInfos.FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<AccountInfo> UpdateAccountInfo(AccountInfo account)
    {
        ArgumentNullException.ThrowIfNull(account);
        
        _context.AccountInfos.Update(account);
        await _context.SaveChangesAsync();
        return account;
    }
}