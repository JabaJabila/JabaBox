namespace JabaBox.WebApi.Dto;

public class BaseDirectoryDto
{
    public Guid Id { get; set; }
    public long BytesOccupied { get; set; }
    public IReadOnlyCollection<string> Directories { get; set; }
}