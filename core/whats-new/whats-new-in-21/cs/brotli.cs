using System;
using System.IO;
using System.IO.Compression;

public static class BrotliExample
{
    // <Snippet1>
    public static Stream DecompressWithBrotli(Stream toDecompress)   
    {
        MemoryStream decompressedStream = new MemoryStream();
        using (BrotliStream decompressionStream = new BrotliStream(toDecompress, CompressionMode.Decompress))
        {
            decompressionStream.CopyTo(decompressedStream);
        }
        return decompressedStream;
    }
    // </Snippet1>
}