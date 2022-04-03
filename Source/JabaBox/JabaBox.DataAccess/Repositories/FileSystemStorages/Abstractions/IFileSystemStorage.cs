﻿using JabaBox.Core.RepositoryAbstractions;

namespace JabaBox.DataAccess.Repositories.FileSystemStorages.Abstractions;

public interface IFileSystemStorage : IFileSystemBaseDirectoryStorage, IFileSystemStorageDirectoryStorage, IFileSystemStorageFileStorage
{
}