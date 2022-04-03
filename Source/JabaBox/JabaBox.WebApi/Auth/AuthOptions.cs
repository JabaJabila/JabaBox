using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace JabaBox.WebApi.Auth;

public class AuthOptions
{
    public const string Issuer = "JabaBox.Server";
    public const string Audience = "JabaBox.Client";
    private const string Key = "supermegaduperpupersecretkey)))";
    public const int Lifetime = 60;
    public static SymmetricSecurityKey GetSymmetricSecurityKey()
    {
        return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Key));
    }
}