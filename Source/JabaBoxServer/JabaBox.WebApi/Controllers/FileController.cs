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
[Route("me/storage")]
public class FileController
{
    private readonly IAccountService _accountService;
    private readonly IStorageService _storageService;
    private readonly IStorageDirectoryMapper _storageDirectoryMapper;
    private readonly IStorageFileMapper _storageFileMapper;
    private readonly ICompressor _compressor;
    private readonly ILogger<FileController> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public FileController(
        IAccountService accountService,
        IStorageService storageService,
        IStorageDirectoryMapper storageDirectoryMapper, 
        IStorageFileMapper storageFileMapper, ICompressor compressor,
        ILogger<FileController> logger, 
        IHttpContextAccessor httpContextAccessor)
    {
        _accountService = accountService ?? throw new ArgumentNullException(nameof(accountService));
        _storageService = storageService ?? throw new ArgumentNullException(nameof(storageService));
        _storageDirectoryMapper =
            storageDirectoryMapper ?? throw new ArgumentNullException(nameof(storageDirectoryMapper));
        _storageFileMapper = storageFileMapper ?? throw new ArgumentNullException(nameof(storageFileMapper));
        _compressor = compressor ?? throw new ArgumentNullException(nameof(compressor));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
    }

    [HttpGet("{directoryName}")]
    public ActionResult GetFiles(string directoryName)
    {
        string login = _httpContextAccessor.HttpContext.User.Identity.Name;
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
                
            _logger.LogInformation("File was found");
            return new OkObjectResult(_storageDirectoryMapper.EntityToDto(storageDirectory));
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
    
    [HttpPost("{directoryName}/upload")]
    public ActionResult UploadFile(string directoryName, IFormFile file, bool compress = false)
    {
        string login = _httpContextAccessor.HttpContext.User.Identity.Name;
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
                _logger.LogInformation("file was compressed before saving");
            }

            StorageFile storageFile = _storageService.AddFile(
                accountInfo, storageDirectory, state, file.FileName, fileBytes);
            
            _logger.LogInformation("file was uploaded");
            return new OkObjectResult(_storageFileMapper.EntityToDto(storageFile));
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
    
    [HttpPut("{directoryName}/{fileName}/rename")]
    public ActionResult RenameFile(string directoryName, string fileName, string newName)
    {
        string login = _httpContextAccessor.HttpContext.User.Identity.Name;
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
            _logger.LogInformation("file was renamed");
            return new OkObjectResult(_storageFileMapper.EntityToDto(storageFile));
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
    
    [HttpDelete("{directoryName}/{fileName}/delete")]
    public ActionResult DeleteFile(string directoryName, string fileName)
    {
        string login = _httpContextAccessor.HttpContext.User.Identity.Name;
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
            _logger.LogInformation("File was deleted");
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
    
    [HttpGet("{directoryName}/{fileName}/download")]
    public ActionResult DownloadFile(string directoryName, string fileName)
    {
        string login = _httpContextAccessor.HttpContext.User.Identity.Name;
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
                _logger.LogInformation("Decompressed file");
            }

            var result = new FileStreamResult(new MemoryStream(data), "application/octet-stream")
            {
                FileDownloadName = fileName,
            };
            
            _logger.LogInformation("file found and ready to download");
            return result;
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