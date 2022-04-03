using System.IO.Compression;
using JabaBox.WebApi.Tools.Compressors.Abstractions;

namespace JabaBox.WebApi.Tools.Compressors.Implementations;

public class OptimalCompressor : ICompressor
{
    public byte[] Compress(byte[] data)
    {
        var output = new MemoryStream();
        using (var dstream = new DeflateStream(output, CompressionLevel.Optimal))
            dstream.Write(data, 0, data.Length);
        
        return output.ToArray();
    }

    public byte[] Decompress(byte[] compressedData)
    {
        var input = new MemoryStream(compressedData);
        var output = new MemoryStream();
        using (var dstream = new DeflateStream(input, CompressionMode.Decompress))
            dstream.CopyTo(output);
        
        return output.ToArray();
    }
}