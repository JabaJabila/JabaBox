namespace JabaBox.WebApi.Dto;

public class AccountInfoDto
{
    public string Login { get; set; }
    public string Password { get; set; }
    public Guid Id { get; set; }
    public int GigabytesAvailable { get; set; }
}