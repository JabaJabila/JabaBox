using JabaBox.Core.Domain.ServicesAbstractions;
using JabaBox.Core.RepositoryAbstractions;
using JabaBox.Core.Services;
using JabaBox.WebApi.Mappers.Abstractions;
using JabaBox.WebApi.Mappers.Implementations;
using JabaBoxServer.DataAccess.DataBaseContexts;
using JabaBoxServer.DataAccess.Repositories;
using JabaBoxServer.DataAccess.Repositories.FileSystemStorages.Abstractions;
using JabaBoxServer.DataAccess.Repositories.FileSystemStorages.Implementations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<JabaBoxDbContext>(opt =>
{
    opt.UseSqlServer(builder.Configuration.GetConnectionString("MyServer"));
});

builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IStorageService, StorageService>();
builder.Services.AddScoped<IAccountInfoMapper, AccountInfoMapper>();
builder.Services.AddScoped<IBaseDirectoryMapper, BaseDirectoryMapper>();
builder.Services.AddScoped<IStorageDirectoryMapper, StorageDirectoryMapper>();
builder.Services.AddScoped<IStorageFileMapper, StorageFileMapper>();
builder.Services.AddScoped<IAccountInfoRepository, AccountInfoRepository>();
builder.Services.AddScoped<IBaseDirectoryRepository, BaseDirectoryRepository>();
builder.Services.AddScoped<IStorageDirectoryRepository, StorageDirectoryRepository>();
builder.Services.AddScoped<IStorageFileRepository, StorageFileRepository>();

var storage = new FileSystemStorage(builder.Configuration.GetValue<string>("FileStoragePath"));
builder.Services.AddScoped<IFileSystemStorage>(_ => storage);
builder.Services.AddScoped<IFileSystemStorageFileStorage>(_ => storage);
builder.Services.AddScoped<IFileSystemBaseDirectoryStorage>(_ => storage);
builder.Services.AddScoped<IFileSystemStorageDirectoryStorage>(_ => storage);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();