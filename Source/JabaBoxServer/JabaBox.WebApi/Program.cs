using JabaBox.Core.Domain.ServicesAbstractions;
using JabaBox.Core.RepositoryAbstractions;
using JabaBox.Core.Services;
using JabaBox.WebApi.Auth;
using JabaBox.WebApi.Mappers.Abstractions;
using JabaBox.WebApi.Mappers.Implementations;
using JabaBox.WebApi.Tools.Compressors.Abstractions;
using JabaBox.WebApi.Tools.Compressors.Implementations;
using JabaBoxServer.DataAccess.DataBaseContexts;
using JabaBoxServer.DataAccess.Repositories;
using JabaBoxServer.DataAccess.Repositories.FileSystemStorages.Abstractions;
using JabaBoxServer.DataAccess.Repositories.FileSystemStorages.Implementations;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NLog;
using NLog.Web;

var builder = WebApplication.CreateBuilder(args);
var logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
logger.Debug("debug");

try
{
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo { 
            Title = "JabaBox.WebApi", 
            Version = "v1" 
        });
        c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.Http,
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Scheme = "bearer",
            Description = "Please insert JWT token into field"
        });

        c.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                new string[] { }
            }
        });
    });


    builder.Logging.ClearProviders();
    builder.Logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
    builder.Host.UseNLog();

    builder.Services.AddDbContext<JabaBoxDbContext>(opt =>
    {
        opt.UseSqlServer(builder.Configuration.GetConnectionString("MyServer"));
    });
    builder.Services.AddHttpContextAccessor();
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
    builder.Services.AddScoped<ICompressor, OptimalCompressor>();

    var storage = new FileSystemStorage(builder.Configuration.GetValue<string>("FileStoragePath"));
    builder.Services.AddScoped<IFileSystemStorage>(_ => storage);
    builder.Services.AddScoped<IFileSystemStorageFileStorage>(_ => storage);
    builder.Services.AddScoped<IFileSystemBaseDirectoryStorage>(_ => storage);
    builder.Services.AddScoped<IFileSystemStorageDirectoryStorage>(_ => storage);
    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.RequireHttpsMetadata = false;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = AuthOptions.Issuer,
                ValidateAudience = true,
                ValidAudience = AuthOptions.Audience,
                ValidateLifetime = true,
                IssuerSigningKey = AuthOptions.GetSymmetricSecurityKey(),
                ValidateIssuerSigningKey = true,
            };
        });
    
    builder.Services.AddAuthorization(options =>
    {
        options.DefaultPolicy = new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme)
            .RequireAuthenticatedUser()
            .Build();
    });

    var app = builder.Build();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();
    app.UseAuthentication();
    app.UseAuthorization();
    app.MapControllers();
    app.Run();
}
catch (Exception e)
{
    logger.Error(e, "Some error stopped the program");
    throw;
}
finally
{
    LogManager.Shutdown();
}

