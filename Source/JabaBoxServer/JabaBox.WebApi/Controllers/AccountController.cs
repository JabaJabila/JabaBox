using System.Net;
using JabaBox.Core.Domain.Entities;
using JabaBox.Core.Domain.Exceptions;
using JabaBox.Core.Domain.ServicesAbstractions;
using JabaBox.WebApi.Mappers.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace JabaBox.WebApi.Controllers;

[ApiController]
[Route("/account")]
public class AccountController
{
    private readonly IAccountService _accountService;
    private readonly IAccountInfoMapper _accountMapper;
    private readonly ILogger<AccountController> _logger;

    public AccountController(
        IAccountService accountService,
        IAccountInfoMapper accountMapper,
        ILogger<AccountController> logger)
    {
        _accountService = accountService ?? throw new ArgumentNullException(nameof(accountService));
        _accountMapper = accountMapper ?? throw new ArgumentNullException(nameof(accountMapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [HttpGet("find-account")]
    public ActionResult FindAccount(string login)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(login))
                throw new ArgumentNullException(nameof(login));

            AccountInfo accountInfo = _accountService.GetAccount(login);
            _logger.LogInformation("Account was successfully found");
            return new OkObjectResult(_accountMapper.EntityToDto(accountInfo));
        }
        catch (JabaBoxException e)
        {
            _logger.LogInformation(e, "");
            return new NotFoundObjectResult(e.Message);
        } 
        catch (Exception e)
        {
            _logger.LogError(e, "");
            return new StatusCodeResult((int) HttpStatusCode.BadRequest);
        }
    }
    
    [HttpPost("register-account")]
    public ActionResult RegisterAccount(string login, string password, int gigabytePlan)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(login))
                throw new ArgumentNullException(nameof(login));
            
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentNullException(nameof(password));

            if (gigabytePlan <= 0)
                throw new AccountInfoException("Gigabyte plan can't be <= 0 gigabytes");
            
            var account = _accountService.RegisterAccount(login, password, gigabytePlan);
            _logger.LogInformation("Account was successfully created");
            return new OkObjectResult(_accountMapper.EntityToDto(account));
        }
        catch (JabaBoxException e)
        {
            _logger.LogInformation(e, "");
            return new NotFoundObjectResult(e.Message);
        } 
        catch (Exception e)
        {
            _logger.LogError(e, "");
            return new StatusCodeResult((int) HttpStatusCode.BadRequest);
        }
    }
    
    [HttpPut("{login}/change-password")]
    public ActionResult ChangePassword(string login, string password, string newPassword)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(login))
                throw new ArgumentNullException(nameof(login));
            
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentNullException(nameof(password));

            if (string.IsNullOrWhiteSpace(newPassword))
                throw new ArgumentNullException(nameof(password));
            
            var account = _accountService.ChangePassword(login, password, newPassword);
            _logger.LogInformation("Password was changed");
            return new OkObjectResult(_accountMapper.EntityToDto(account));
        }
        catch (JabaBoxException e)
        {
            _logger.LogInformation(e, "");
            return new NotFoundObjectResult(e.Message);
        } 
        catch (Exception e)
        {
            _logger.LogError(e, "");
            return new StatusCodeResult((int) HttpStatusCode.BadRequest);
        }
    }
    
    [HttpPut("{login}/change-plan")]
    public ActionResult ChangeGigabytePlan(string login, int newGigabytes)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(login))
                throw new ArgumentNullException(nameof(login));

            if (newGigabytes <= 0)
                throw new AccountInfoException("Impossible to change gigabyte plan to <= 0 gigabytes");

            var account = _accountService.ChangeGigabytesPlan(login, newGigabytes);
            _logger.LogInformation("Gigabyte plan was changed");
            
            return new OkObjectResult(_accountMapper.EntityToDto(account));
        }
        catch (JabaBoxException e)
        {
            _logger.LogInformation(e, "");
            return new NotFoundObjectResult(e.Message);
        } 
        catch (Exception e)
        {
            _logger.LogError(e, "");
            return new StatusCodeResult((int) HttpStatusCode.BadRequest);
        }
    }
}