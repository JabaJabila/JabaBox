using System.Linq;
using JabaBox.Core.Domain.Exceptions;
using JabaBox.Core.Domain.ServicesAbstractions;
using JabaBox.Core.Services;
using JabaBox.UnitTests.DbContextsForTests;
using JabaBox.UnitTests.RepositoriesForTests;
using NUnit.Framework;

namespace JabaBox.UnitTests;

[TestFixture]
public class AccountServiceTests
{
    private IAccountService _accountService;
    private JabaBoxDbTestContext _context;

    [SetUp]
    public void Setup()
    {
        var context = new JabaBoxDbTestContext();
        _accountService = new AccountService(new InMemoryAccountInfoRepository(context), 
            new InMemoryBaseDirectoryRepository(context));
        _context = context;
    }

    [TestCase("admin", "12345", 1)]
    [TestCase("jaba", "qwerty", 10)]
    public void CreateAccount_AccountCreated(string login, string password, int gigabytes)
    {
        var account = _accountService.RegisterAccount(login, password, gigabytes);
        Assert.Contains(account, _context.AccountInfos.ToList());
        var foundAcc = _accountService.GetAccount(login);
        Assert.True(foundAcc.Login == login && foundAcc.Password == password && foundAcc.GigabytesAvailable == gigabytes);
        Assert.NotNull(_context.BaseDirectories.First(d => d.UserId == account.Id));
    }

    [TestCase("admin", "12345", 1, "7777")]
    [TestCase("jaba", "qwerty", 10, "ytrewq")]
    public void CreateAccountWhichLoginExists_ThrowsException(
        string login,
        string password,
        int gigabytes,
        string differentPassword)
    {
        _accountService.RegisterAccount(login, password, gigabytes);
        Assert.Throws<AccountInfoException>(() =>
        {
            _accountService.RegisterAccount(login, differentPassword, gigabytes);
        });
    }
    
    [TestCase("admin", "12345", 1, "7777")]
    [TestCase("jaba", "qwerty", 10, "ytrewq")]
    public void CreateAccountChangePassword_PasswordChanges(
        string login,
        string password,
        int gigabytes,
        string newPassword)
    {
        var account = _accountService.RegisterAccount(login, password, gigabytes);
        _accountService.ChangePassword(login, password, newPassword);
        var foundAcc = _accountService.GetAccount(login);
        
        Assert.True(foundAcc.Login == login && foundAcc.Password == newPassword && foundAcc.GigabytesAvailable == gigabytes);
    }
    
    [TestCase("admin", "12345", 1, "7777")]
    [TestCase("jaba", "qwerty", 10, "ytrewq")]
    public void CreateAccountChangePasswordWrongOldPassword_ThrowsException(
        string login,
        string password,
        int gigabytes,
        string newPassword)
    {
        var account = _accountService.RegisterAccount(login, password, gigabytes);
        Assert.Throws<AccountInfoException>(() =>
        {
            _accountService.ChangePassword(login, password + "1", newPassword);
        });
    }
    
    [TestCase("admin", "12345", 1, 2)]
    [TestCase("jaba", "qwerty", 10, 2)]
    public void CreateAccountChangeGigabytePlan_PlanChanged(
        string login,
        string password,
        int gigabytes,
        int newGigabytes)
    {
        var account = _accountService.RegisterAccount(login, password, gigabytes);
        _accountService.ChangeGigabytesPlan(login, newGigabytes);
        var foundAcc = _accountService.GetAccount(login);
        
        Assert.True(foundAcc.Login == login && foundAcc.Password == password && foundAcc.GigabytesAvailable == newGigabytes);
    }
    
    [TestCase("admin", "12345", 1, 0)]
    [TestCase("jaba", "qwerty", 10, -2)]
    public void CreateAccountChangeGigabytePlanWrongGigabytes_ThrowsException(
        string login,
        string password,
        int gigabytes,
        int newGigabytes)
    {
        _accountService.RegisterAccount(login, password, gigabytes);
        
        Assert.Throws<AccountInfoException>(() =>
        {
            _accountService.ChangeGigabytesPlan(login, newGigabytes);
        });
    }
}