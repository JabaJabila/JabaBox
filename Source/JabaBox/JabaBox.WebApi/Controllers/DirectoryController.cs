using System.ComponentModel.DataAnnotations;
using System.Net;
using JabaBox.Core.Domain.Entities;
using JabaBox.Core.Domain.Exceptions;
using JabaBox.Core.Domain.ServicesAbstractions;
using JabaBox.WebApi.Mappers.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JabaBox.WebApi.Controllers;

[ApiController]
[Route("me/storage")]
public class DirectoryController
{
    private readonly IAccountService _accountService;
    private readonly IStorageService _storageService;
    private readonly IBaseDirectoryMapper _baseDirectoryMapper;
    private readonly IStorageDirectoryMapper _storageDirectoryMapper;
    private readonly ILogger<DirectoryController> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public DirectoryController(
        IAccountService accountService,
        IStorageService storageService,
        IBaseDirectoryMapper baseDirectoryMapper, 
        IStorageDirectoryMapper storageDirectoryMapper, 
        ILogger<DirectoryController> logger, 
        IHttpContextAccessor httpContextAccessor)
    {
        _accountService = accountService ?? throw new ArgumentNullException(nameof(accountService));
        _storageService = storageService ?? throw new ArgumentNullException(nameof(storageService));
        _baseDirectoryMapper = baseDirectoryMapper ?? throw new ArgumentNullException(nameof(baseDirectoryMapper));
        _storageDirectoryMapper = storageDirectoryMapper ?? throw new ArgumentNullException(nameof(storageDirectoryMapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
    }

    [Authorize(AuthenticationSchemes = "Bearer")]
    [HttpGet("directories")]
    public ActionResult GetDirectories()
    {
        string login = _httpContextAccessor.HttpContext.User.Identity.Name;
        try
        {
            if (string.IsNullOrWhiteSpace(login))
                throw new ArgumentNullException(nameof(login));

            AccountInfo accountInfo = _accountService.GetAccount(login);
            BaseDirectory baseDirectory = _storageService.GetBaseDirectory(accountInfo);
            _logger.LogInformation("Directory found");
            return new OkObjectResult(_baseDirectoryMapper.EntityToDto(baseDirectory));
        }
        catch (JabaBoxException e)
        {
            _logger.LogInformation(e, "");
            return new NotFoundObjectResult(e.Message);
        } 
        catch (Exception e)
        {
            _logger.LogInformation(e, "");
            return new StatusCodeResult((int) HttpStatusCode.BadRequest);
        }
    }
    
    [Authorize(AuthenticationSchemes = "Bearer")]
    [HttpPost("directory-create")]
    public ActionResult CreateDirectory([Required] string name)
    {
        string login = _httpContextAccessor.HttpContext.User.Identity.Name;
        try
        {
            if (string.IsNullOrWhiteSpace(login))
                throw new ArgumentNullException(nameof(login));
            
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));

            AccountInfo accountInfo = _accountService.GetAccount(login);
            StorageDirectory directory = _storageService.CreateDirectory(accountInfo, name);
            _logger.LogInformation("Directory was created");
            return new OkObjectResult(_storageDirectoryMapper.EntityToDto(directory));
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
    [HttpPut("{name}/rename")]
    public ActionResult RenameDirectory([Required] string name, [Required] string newName)
    {
        string login = _httpContextAccessor.HttpContext.User.Identity.Name;
        try
        {
            if (string.IsNullOrWhiteSpace(login))
                throw new ArgumentNullException(nameof(login));
            
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));
            
            if (string.IsNullOrWhiteSpace(newName))
                throw new ArgumentNullException(nameof(newName));

            AccountInfo accountInfo = _accountService.GetAccount(login);
            StorageDirectory? directory = _storageService.FindDirectory(accountInfo, name);
            if (directory is null)
                throw new DirectoryException($"Directory \'{name}\' not found");
                
            directory = _storageService.RenameDirectory(accountInfo, directory, newName);
            _logger.LogInformation("Directory was renamed");
            return new OkObjectResult(_storageDirectoryMapper.EntityToDto(directory));
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
    [HttpDelete("{name}/delete")]
    public ActionResult DeleteDirectory([Required] string name)
    {
        string login = _httpContextAccessor.HttpContext.User.Identity.Name;
        try
        {
            if (string.IsNullOrWhiteSpace(login))
                throw new ArgumentNullException(nameof(login));
            
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));

            AccountInfo accountInfo = _accountService.GetAccount(login);
            StorageDirectory? directory = _storageService.FindDirectory(accountInfo, name);
            if (directory is null)
                throw new DirectoryException($"Directory \'{name}\' not found");

            _storageService.DeleteDirectory(accountInfo, directory);
            _logger.LogInformation("Directory was deleted");
            return new OkResult();
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