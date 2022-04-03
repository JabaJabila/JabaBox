using System.Net;
using JabaBox.Core.Domain.Entities;
using JabaBox.Core.Domain.Exceptions;
using JabaBox.Core.Domain.ServicesAbstractions;
using JabaBox.WebApi.Mappers.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace JabaBox.WebApi.Controllers;

[ApiController]
[Route("{login}/storage")]
public class DirectoryController
{
    private readonly IAccountService _accountService;
    private readonly IStorageService _storageService;
    private readonly IBaseDirectoryMapper _baseDirectoryMapper;
    private readonly IStorageDirectoryMapper _storageDirectoryMapper;

    public DirectoryController(
        IAccountService accountService,
        IStorageService storageService,
        IBaseDirectoryMapper baseDirectoryMapper, 
        IStorageDirectoryMapper storageDirectoryMapper)
    {
        _accountService = accountService ?? throw new ArgumentNullException(nameof(accountService));
        _storageService = storageService ?? throw new ArgumentNullException(nameof(storageService));
        _baseDirectoryMapper = baseDirectoryMapper ?? throw new ArgumentNullException(nameof(baseDirectoryMapper));
        _storageDirectoryMapper = storageDirectoryMapper ?? throw new ArgumentNullException(nameof(storageDirectoryMapper));
    }

    [HttpGet("directories")]
    public ActionResult GetDirectories(string login)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(login))
                throw new ArgumentNullException(nameof(login));

            AccountInfo accountInfo = _accountService.GetAccount(login);
            BaseDirectory baseDirectory = _storageService.GetBaseDirectory(accountInfo);
            return new OkObjectResult(_baseDirectoryMapper.EntityToDto(baseDirectory));
        }
        catch (JabaBoxException e)
        {
            return new NotFoundObjectResult(e.Message);
        } 
        catch (Exception)
        {
            return new StatusCodeResult((int) HttpStatusCode.BadRequest);
        }
    }
    
    [HttpPost("directory-create")]
    public ActionResult CreateDirectory(string login, string name)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(login))
                throw new ArgumentNullException(nameof(login));
            
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));

            AccountInfo accountInfo = _accountService.GetAccount(login);
            StorageDirectory directory = _storageService.CreateDirectory(accountInfo, name);
            return new OkObjectResult(_storageDirectoryMapper.EntityToDto(directory));
        }
        catch (JabaBoxException e)
        {
            return new NotFoundObjectResult(e.Message);
        } 
        catch (Exception)
        {
            return new StatusCodeResult((int) HttpStatusCode.BadRequest);
        }
    }
    
    [HttpPut("{name}/rename")]
    public ActionResult RenameDirectory(string login, string name, string newName)
    {
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
            return new OkObjectResult(_storageDirectoryMapper.EntityToDto(directory));
        }
        catch (JabaBoxException e)
        {
            return new NotFoundObjectResult(e.Message);
        } 
        catch (Exception)
        {
            return new StatusCodeResult((int) HttpStatusCode.BadRequest);
        }
    }
    
    [HttpDelete("{name}/delete")]
    public ActionResult DeleteDirectory(string login, string name)
    {
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
            return new OkResult();
        }
        catch (JabaBoxException e)
        {
            return new NotFoundObjectResult(e.Message);
        } 
        catch (Exception)
        {
            return new StatusCodeResult((int) HttpStatusCode.BadRequest);
        }
    }
}