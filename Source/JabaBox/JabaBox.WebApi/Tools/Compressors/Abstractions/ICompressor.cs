namespace JabaBox.WebApi.Tools.Compressors.Abstractions;

public interface ICompressor
{
    byte[] Compress(byte[] data);
    byte[] Decompress(byte[] compressedData);
}