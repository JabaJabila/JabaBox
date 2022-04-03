using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using JabaBox.Core.Domain.Entities;
using JabaBox.Core.Domain.Exceptions;
using JabaBox.Core.Domain.ServicesAbstractions;
using JabaBox.WebApi.Auth;
using JabaBox.WebApi.Mappers.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace JabaBox.WebApi.Controllers;

[ApiController]
[Route("/account")]
public class AccountController
{
    private readonly IAccountService _accountService;
    private readonly IAccountInfoMapper _accountMapper;
    private readonly ILogger<AccountController> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AccountController(
        IAccountService accountService,
        IAccountInfoMapper accountMapper,
        ILogger<AccountController> logger, 
        IHttpContextAccessor httpContextAccessor)
    {
        _accountService = accountService ?? throw new ArgumentNullException(nameof(accountService));
        _accountMapper = accountMapper ?? throw new ArgumentNullException(nameof(accountMapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
    }
    
    [HttpPost("/get-login-token")]
    public IActionResult GetToken([Required] string login, [Required] string password)
    {
        try
        {
            var identity = GetIdentity(login, password);

            var now = DateTime.UtcNow;
            var jwt = new JwtSecurityToken(
                issuer: AuthOptions.Issuer,
                audience: AuthOptions.Audience,
                notBefore: now,
                claims: identity.Claims,
                expires: now.Add(TimeSpan.FromMinutes(AuthOptions.Lifetime)),
                signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(),
                    SecurityAlgorithms.HmacSha256));
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);
 
            var response = new
            {
                access_token = encodedJwt,
                username = identity.Name
            };

            _logger.LogInformation("Successfully logged in");
            return new OkObjectResult(response);
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

    [Authorize(AuthenticationSchemes = "Bearer")]
    [HttpGet("me")]
    public ActionResult AccountInfo()
    {
        string login = _httpContextAccessor.HttpContext.User.Identity.Name;
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
    public ActionResult RegisterAccount([Required] string login, [Required]  string password, [Required]  int gigabytePlan)
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
    
    [Authorize(AuthenticationSchemes = "Bearer")]
    [HttpPut("me/change-password")]
    public ActionResult ChangePassword([Required] string password, [Required] string newPassword)
    {
        string login = _httpContextAccessor.HttpContext.User.Identity.Name;
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
    
    [Authorize(AuthenticationSchemes = "Bearer")]
    [HttpPut("me/change-plan")]
    public ActionResult ChangeGigabytePlan([Required] int newGigabytes)
    {
        string login = _httpContextAccessor.HttpContext.User.Identity.Name;
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
    
    private ClaimsIdentity GetIdentity(string login, string password)
    {
        var account = _accountService.GetAccount(login);
        if (account.Password != password)
            throw new AccountInfoException("Wrong login or password");
        
        var claims = new List<Claim> {
                new Claim(ClaimsIdentity.DefaultNameClaimType, account.Login),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, "user")
        };
        
        var claimsIdentity = new ClaimsIdentity(
            claims, "Token", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
        
        return claimsIdentity;
    }
}