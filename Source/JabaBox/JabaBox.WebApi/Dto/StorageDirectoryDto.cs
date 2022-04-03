namespace JabaBox.WebApi.Dto;

public class StorageDirectoryDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public IReadOnlyCollection<StorageFileDto> Files { get; set; }
}