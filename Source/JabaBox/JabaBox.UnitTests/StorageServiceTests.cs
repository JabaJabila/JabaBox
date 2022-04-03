using System.Linq;
using JabaBox.Core.Domain.Enums;
using JabaBox.Core.Domain.Exceptions;
using JabaBox.Core.Domain.ServicesAbstractions;
using JabaBox.Core.Services;
using JabaBox.UnitTests.DbContextsForTests;
using JabaBox.UnitTests.RepositoriesForTests;
using NUnit.Framework;

namespace JabaBox.UnitTests;

[TestFixture]
public class StorageServiceTests
{
    private IAccountService _accountService;
    private IStorageService _storageService;
    private JabaBoxDbTestContext _context;

    [SetUp]
    public void Setup()
    {
        var context = new JabaBoxDbTestContext();
        var baseDirectory = new InMemoryBaseDirectoryRepository(context);
        _accountService = new AccountService(
            new InMemoryAccountInfoRepository(context), 
            baseDirectory);
        _storageService = new StorageService(baseDirectory,
            new InMemoryStorageDirectoryRepository(context),
            new InMemoryStorageFileRepository(context));
        _context = context;
    }

    [TestCase("admin", "12345", 1, "directory")]
    [TestCase("jaba", "qwerty", 10, "docs")]
    public void CreateDirectory_DirectoryCreated(string login, string password, int gigabytes, string dirName)
    {
        var account = _accountService.RegisterAccount(login, password, gigabytes);
        var dir = _storageService.CreateDirectory(account, dirName);
        var dirFound = _storageService.FindDirectory(account, dirName);
        Assert.AreEqual((long)gigabytes * 1024 * 1024 * 1024, _storageService.BytesAvailable(account));
        Assert.True(dirFound is not null && dirFound.Name == dir.Name && dir.Id == dirFound.Id);
    }
    
    [TestCase("admin", "12345", 1, "directory")]
    [TestCase("jaba", "qwerty", 10, "docs")]
    public void CreateDirectoryThatAlreadyExist_ThrowsException(
        string login,
        string password,
        int gigabytes,
        string dirName)
    {
        var account = _accountService.RegisterAccount(login, password, gigabytes);
        _storageService.CreateDirectory(account, dirName);
        Assert.Throws<DirectoryException>(() =>
        {
            _storageService.CreateDirectory(account, dirName);
        });
    }
    
    [TestCase("admin", "12345", 1, "directory", "dir")]
    [TestCase("jaba", "qwerty", 10, "docs", "documents")]
    public void RenameDirectory_DirectoryRenamed(
        string login,
        string password,
        int gigabytes,
        string dirName,
        string newDirName)
    {
        var account = _accountService.RegisterAccount(login, password, gigabytes);
        var dir = _storageService.CreateDirectory(account, dirName);
        dir = _storageService.RenameDirectory(account, dir, newDirName);
        var dirFound = _storageService.FindDirectory(account, newDirName);
        Assert.True(dirFound is not null && dirFound.Name == dir.Name && dir.Id == dirFound.Id);
    }
    
    [TestCase("admin", "12345", 1, "directory", "dir")]
    [TestCase("jaba", "qwerty", 10, "docs", "documents")]
    public void RenameDirectoryToExistedName_ThrowsException(
        string login,
        string password,
        int gigabytes,
        string dirName,
        string newDirName)
    {
        var account = _accountService.RegisterAccount(login, password, gigabytes);
        var dir = _storageService.CreateDirectory(account, dirName);
        _storageService.CreateDirectory(account, newDirName);
        Assert.Throws<DirectoryException>(() =>
        {
            _storageService.RenameDirectory(account, dir, newDirName);
        });
    }
    
    [TestCase("admin", "12345", 1, "directory")]
    [TestCase("jaba", "qwerty", 10, "docs")]
    public void DeleteDirectory_DirectoryDeleted(
        string login,
        string password,
        int gigabytes,
        string dirName)
    {
        var account = _accountService.RegisterAccount(login, password, gigabytes);
        var dir = _storageService.CreateDirectory(account, dirName);
        _storageService.DeleteDirectory(account, dir);
        Assert.Null(_storageService.FindDirectory(account, dirName));
    }
    
    [TestCase("admin", "12345", 1, "directory")]
    [TestCase("jaba", "qwerty", 10, "docs")]
    public void DeleteOtherUserDirectory_ThrowsException(
        string login,
        string password,
        int gigabytes,
        string dirName)
    {
        var account1 = _accountService.RegisterAccount(login, password, gigabytes);
        var account2 = _accountService.RegisterAccount(login + "2", password, gigabytes);
        var dir = _storageService.CreateDirectory(account1, dirName);
        Assert.Throws<DirectoryException>(() =>
        {
            _storageService.DeleteDirectory(account2, dir);
        });
    }
    
    [TestCase("admin", "12345", 1, "directory", "t1.txt", 
        new byte[] {0x20, 0x12, 0x20, 0x12, 0x20, 0x12})]
    [TestCase("jaba", "qwerty", 10, "docs", "doc1.docx",
        new byte[] {0x13, 0x11, 0x20, 0x12, 0x13, 0x11, 0x20, 0x12, 0x13, 0x11, 0x20, 0x12})]
    public void CreateFile_FileCreated(
        string login, 
        string password, 
        int gigabytes, 
        string dirName, 
        string fileName, 
        byte[] data)
    {
        var account = _accountService.RegisterAccount(login, password, gigabytes);
        var dir = _storageService.CreateDirectory(account, dirName);
        var file = _storageService.AddFile(account, dir, FileState.Normal, fileName, data);
        Assert.AreEqual((long)gigabytes * 1024 * 1024 * 1024 - data.Length, _storageService.BytesAvailable(account));
        var fileFound = _storageService.FindFile(account, dir, fileName);
        Assert.True(fileFound is not null && fileFound.Name == file.Name && fileFound.Id == file.Id);
    }
    
    [TestCase("admin", "12345", 1, "directory", "t1.txt", 
        new byte[] {0x20, 0x12, 0x20, 0x12, 0x20, 0x12})]
    [TestCase("jaba", "qwerty", 10, "docs", "doc1.docx",
        new byte[] {0x13, 0x11, 0x20, 0x12, 0x13, 0x11, 0x20, 0x12, 0x13, 0x11, 0x20, 0x12})]
    public void CreateFileWhichExists_ThrowsException(
        string login, 
        string password, 
        int gigabytes, 
        string dirName, 
        string fileName, 
        byte[] data)
    {
        var account = _accountService.RegisterAccount(login, password, gigabytes);
        var dir = _storageService.CreateDirectory(account, dirName);
        var file = _storageService.AddFile(account, dir, FileState.Normal, fileName, data);
        Assert.Throws<FileException>(() =>
        {
            _storageService.AddFile(account, dir, FileState.Normal, fileName, data);
        });
    }
    
    // TEST THIS WITH 1 GIGABYTE AVAILABLE
    [TestCase("admin", "12345", 1, "directory", "t1.txt", 
        new byte[] {0x20, 0x12, 0x20, 0x12, 0x20, 0x12})]
    [TestCase("jaba", "qwerty", 1, "docs", "doc1.docx",
        new byte[] {0x13, 0x11, 0x20, 0x12, 0x13, 0x11, 0x20, 0x12, 0x13, 0x11, 0x20, 0x12})]
    public void CreateFileNotEnoughSpace_ThrowsException(
        string login, 
        string password, 
        int gigabytes, 
        string dirName, 
        string fileName, 
        byte[] data)
    {
        var account = _accountService.RegisterAccount(login, password, gigabytes);
        var dir = _storageService.CreateDirectory(account, dirName);
        var dataList = data.ToList();
        while (dataList.Count <= _storageService.BytesAvailable(account))
            dataList.AddRange(dataList);

        data = dataList.ToArray();
        Assert.Throws<FileException>(() =>
        {
            _storageService.AddFile(account, dir, FileState.Normal, fileName, data);
        });
    }
    
    [TestCase("admin", "12345", 1, "directory", "t1.txt", 
        new byte[] {0x20, 0x12, 0x20, 0x12, 0x20, 0x12}, "t2.txt")]
    [TestCase("jaba", "qwerty", 1, "docs", "doc1.docx",
        new byte[] {0x13, 0x11, 0x20, 0x12, 0x13, 0x11, 0x20, 0x12, 0x13, 0x11, 0x20, 0x12}, "doc2.docx")]
    public void RenameFile_FileRenamed(
        string login, 
        string password, 
        int gigabytes, 
        string dirName, 
        string fileName, 
        byte[] data,
        string newFileName)
    {
        var account = _accountService.RegisterAccount(login, password, gigabytes);
        var dir = _storageService.CreateDirectory(account, dirName);
        var file = _storageService.AddFile(account, dir, FileState.Normal, fileName, data);
        _storageService.RenameFile(account, dir, file, newFileName);
        file = _storageService.FindFile(account, dir, newFileName);
        Assert.True(file is not null && file.ByteSize == data.Length);
    }
    
    [TestCase("admin", "12345", 1, "directory", "t1.txt", 
        new byte[] {0x20, 0x12, 0x20, 0x12, 0x20, 0x12}, "t2.txt")]
    [TestCase("jaba", "qwerty", 1, "docs", "doc1.docx",
        new byte[] {0x13, 0x11, 0x20, 0x12, 0x13, 0x11, 0x20, 0x12, 0x13, 0x11, 0x20, 0x12}, "doc2.docx")]
    public void RenameFileToFileThatExists_ThrowsException(
        string login, 
        string password, 
        int gigabytes, 
        string dirName, 
        string fileName, 
        byte[] data,
        string newFileName)
    {
        var account = _accountService.RegisterAccount(login, password, gigabytes);
        var dir = _storageService.CreateDirectory(account, dirName);
        var fileToRename = _storageService.AddFile(account, dir, FileState.Normal, fileName, data);
        _storageService.AddFile(account, dir, FileState.Normal, newFileName, data);
        Assert.Throws<FileException>(() =>
        {
            _storageService.RenameFile(account, dir, fileToRename, newFileName);
        });
    }
    
    [TestCase("admin", "12345", 1, "directory", "t1.txt", 
        new byte[] {0x20, 0x12, 0x20, 0x12, 0x20, 0x12})]
    [TestCase("jaba", "qwerty", 1, "docs", "doc1.docx",
        new byte[] {0x13, 0x11, 0x20, 0x12, 0x13, 0x11, 0x20, 0x12, 0x13, 0x11, 0x20, 0x12})]
    public void DeleteFile_FileDeleted(
        string login, 
        string password, 
        int gigabytes, 
        string dirName, 
        string fileName, 
        byte[] data)
    {
        var account = _accountService.RegisterAccount(login, password, gigabytes);
        var dir = _storageService.CreateDirectory(account, dirName);
        var file = _storageService.AddFile(account, dir, FileState.Normal, fileName, data);
        _storageService.DeleteFile(account, dir, file);
        Assert.Null(_storageService.FindFile(account, dir, fileName));
    }
    
    [TestCase("admin", "12345", 1, "directory", "t1.txt", 
        new byte[] {0x20, 0x12, 0x20, 0x12, 0x20, 0x12})]
    [TestCase("jaba", "qwerty", 1, "docs", "doc1.docx",
        new byte[] {0x13, 0x11, 0x20, 0x12, 0x13, 0x11, 0x20, 0x12, 0x13, 0x11, 0x20, 0x12})]
    public void DeleteOtherUserFile_ThrowsException(
        string login, 
        string password, 
        int gigabytes, 
        string dirName, 
        string fileName, 
        byte[] data)
    {
        var account1 = _accountService.RegisterAccount(login, password, gigabytes);
        var account2 = _accountService.RegisterAccount(login + "2", password, gigabytes);
        var dir1 = _storageService.CreateDirectory(account1, dirName);
        var dir2 = _storageService.CreateDirectory(account2, dirName);
        var file = _storageService.AddFile(account1, dir1, FileState.Normal, fileName, data);
        Assert.Throws<FileException>(() =>
        {
            _storageService.DeleteFile(account2, dir2, file);
        });
    }
}