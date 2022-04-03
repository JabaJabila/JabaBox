using System.Net;
using JabaBox.Core.Domain.Entities;
using JabaBox.Core.Domain.Enums;
using JabaBox.Core.Domain.Exceptions;
using JabaBox.Core.Domain.ServicesAbstractions;
using JabaBox.WebApi.Mappers.Abstractions;
using JabaBox.WebApi.Tools.Compressors.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace JabaBox.WebApi.Controllers;

[ApiController]
[Route("{login}/storage/{directoryName}")]
public class FileController
{
    private readonly IAccountService _accountService;
    private readonly IStorageService _storageService;
    private readonly IStorageDirectoryMapper _storageDirectoryMapper;
    private readonly IStorageFileMapper _storageFileMapper;
    private readonly ICompressor _compressor;

    public FileController(
        IAccountService accountService,
        IStorageService storageService,
        IStorageDirectoryMapper storageDirectoryMapper, 
        IStorageFileMapper storageFileMapper, ICompressor compressor)
    {
        _accountService = accountService ?? throw new ArgumentNullException(nameof(accountService));
        _storageService = storageService ?? throw new ArgumentNullException(nameof(storageService));
        _storageDirectoryMapper =
            storageDirectoryMapper ?? throw new ArgumentNullException(nameof(storageDirectoryMapper));
        _storageFileMapper = storageFileMapper ?? throw new ArgumentNullException(nameof(storageFileMapper));
        _compressor = compressor ?? throw new ArgumentNullException(nameof(compressor));
    }

    [HttpGet]
    public ActionResult GetFiles(string login, string directoryName)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(login))
                throw new ArgumentNullException(nameof(login));
            
            if (string.IsNullOrWhiteSpace(directoryName))
                throw new ArgumentNullException(nameof(directoryName));

            AccountInfo accountInfo = _accountService.GetAccount(login);
            StorageDirectory? storageDirectory = _storageService.FindDirectory(accountInfo, directoryName);
            if (storageDirectory is null)
                throw new DirectoryException($"Directory \'{directoryName}\' not found");
                
            return new OkObjectResult(_storageDirectoryMapper.EntityToDto(storageDirectory));
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
    
    [HttpPost("upload")]
    public ActionResult UploadFile(string login, string directoryName, IFormFile file, bool compress = false)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(file);
            
            if (string.IsNullOrWhiteSpace(login))
                throw new ArgumentNullException(nameof(login));
            
            if (string.IsNullOrWhiteSpace(directoryName))
                throw new ArgumentNullException(nameof(directoryName));

            AccountInfo accountInfo = _accountService.GetAccount(login);
            StorageDirectory? storageDirectory = _storageService.FindDirectory(accountInfo, directoryName);
            if (storageDirectory is null)
                throw new DirectoryException($"Directory \'{directoryName}\' not found");

            byte[] fileBytes;
            using (var ms = new MemoryStream())
            {
                file.CopyTo(ms);
                fileBytes = ms.ToArray();
            }

            var state = FileState.Normal;
            if (compress)
            {
                fileBytes = _compressor.Compress(fileBytes);
                state = FileState.Compressed;
            }

            StorageFile storageFile = _storageService.AddFile(
                accountInfo, storageDirectory, state, file.FileName, fileBytes);
            
            return new OkObjectResult(_storageFileMapper.EntityToDto(storageFile));
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
    
    [HttpPut("{fileName}/rename")]
    public ActionResult RenameFile(string login, string directoryName, string fileName, string newName)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(login))
                throw new ArgumentNullException(nameof(login));
            
            if (string.IsNullOrWhiteSpace(directoryName))
                throw new ArgumentNullException(nameof(directoryName));
            
            if (string.IsNullOrWhiteSpace(fileName))
                throw new ArgumentNullException(nameof(fileName));
            
            if (string.IsNullOrWhiteSpace(newName))
                throw new ArgumentNullException(nameof(newName));

            AccountInfo accountInfo = _accountService.GetAccount(login);
            StorageDirectory? storageDirectory = _storageService.FindDirectory(accountInfo, directoryName);
            if (storageDirectory is null)
                throw new DirectoryException($"Directory \'{directoryName}\' not found");

            var storageFile = _storageService.FindFile(accountInfo, storageDirectory, fileName);
            if (storageFile is null)
                throw new FileException($"File \'{fileName}\' not found");

            storageFile = _storageService.RenameFile(accountInfo, storageDirectory, storageFile, newName);
            return new OkObjectResult(_storageFileMapper.EntityToDto(storageFile));
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
    
    [HttpDelete("{fileName}/delete")]
    public ActionResult DeleteFile(string login, string directoryName, string fileName)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(login))
                throw new ArgumentNullException(nameof(login));
            
            if (string.IsNullOrWhiteSpace(directoryName))
                throw new ArgumentNullException(nameof(directoryName));
            
            if (string.IsNullOrWhiteSpace(fileName))
                throw new ArgumentNullException(nameof(fileName));

            AccountInfo accountInfo = _accountService.GetAccount(login);
            StorageDirectory? storageDirectory = _storageService.FindDirectory(accountInfo, directoryName);
            if (storageDirectory is null)
                throw new DirectoryException($"Directory \'{directoryName}\' not found");

            var storageFile = _storageService.FindFile(accountInfo, storageDirectory, fileName);
            if (storageFile is null)
                throw new FileException($"File \'{fileName}\' not found");
            
            _storageService.DeleteFile(accountInfo, storageDirectory, storageFile);
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
    
    [HttpGet("{fileName}/download")]
    public ActionResult DownloadFile(string login, string directoryName, string fileName)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(login))
                throw new ArgumentNullException(nameof(login));

            if (string.IsNullOrWhiteSpace(directoryName))
                throw new ArgumentNullException(nameof(directoryName));

            if (string.IsNullOrWhiteSpace(fileName))
                throw new ArgumentNullException(nameof(fileName));

            AccountInfo accountInfo = _accountService.GetAccount(login);
            StorageDirectory? storageDirectory = _storageService.FindDirectory(accountInfo, directoryName);
            if (storageDirectory is null)
                throw new DirectoryException($"Directory \'{directoryName}\' not found");

            var storageFile = _storageService.FindFile(accountInfo, storageDirectory, fileName);
            if (storageFile is null)
                throw new FileException($"File \'{fileName}\' not found");

            byte[] data = _storageService.GetFileData(accountInfo, storageDirectory, storageFile);

            if (storageFile.State == FileState.Compressed)
            {
                data = _compressor.Decompress(data);
            }

            var result = new FileStreamResult(new MemoryStream(data), "application/octet-stream")
            {
                FileDownloadName = fileName,
            };
            
            return result;
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