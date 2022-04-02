using JabaBox.Core.Domain.ServicesAbstractions;
using Microsoft.AspNetCore.Mvc;

namespace JabaBox.WebApi.Controllers;

[ApiController]
[Route("/account")]
public class AccountController
{
    private readonly IAccountService _accountService;
    // private readonly 
}