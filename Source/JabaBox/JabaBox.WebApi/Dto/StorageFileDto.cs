namespace JabaBox.WebApi.Dto;

public class StorageFileDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string State { get; set; }
    public long Size { get; set; }
}