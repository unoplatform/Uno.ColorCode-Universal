using System;
using System.Buffers.Binary;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace ColorCode.WinUITests
{
    /// <summary>
    /// Minimal PNG encoder, so snapshots need the same code on WinAppSDK and Uno (no BitmapEncoder).
    /// </summary>
    internal static class PngWriter
    {
        public static void WriteBgra(string path, byte[] bgra, int stride, int x, int y, int width, int height)
        {
            // One filter byte (0 = none) per scanline, then RGBA pixels.
            var raw = new byte[height * (1 + width * 4)];
            var o = 0;
            for (var row = 0; row < height; row++)
            {
                raw[o++] = 0;
                for (var col = 0; col < width; col++)
                {
                    var i = ((y + row) * stride + x + col) * 4;
                    raw[o++] = bgra[i + 2];
                    raw[o++] = bgra[i + 1];
                    raw[o++] = bgra[i];
                    raw[o++] = bgra[i + 3];
                }
            }

            using var compressed = new MemoryStream();
            using (var zlib = new ZLibStream(compressed, CompressionLevel.Optimal, leaveOpen: true))
            {
                zlib.Write(raw);
            }

            using var file = File.Create(path);
            file.Write(new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A });

            var header = new byte[13];
            BinaryPrimitives.WriteInt32BigEndian(header.AsSpan(0), width);
            BinaryPrimitives.WriteInt32BigEndian(header.AsSpan(4), height);
            header[8] = 8; // bit depth
            header[9] = 6; // RGBA
            WriteChunk(file, "IHDR", header);
            WriteChunk(file, "IDAT", compressed.ToArray());
            WriteChunk(file, "IEND", Array.Empty<byte>());
        }

        private static void WriteChunk(Stream stream, string type, byte[] data)
        {
            var buffer = new byte[4];
            BinaryPrimitives.WriteInt32BigEndian(buffer, data.Length);
            stream.Write(buffer);

            var typeBytes = Encoding.ASCII.GetBytes(type);
            stream.Write(typeBytes);
            stream.Write(data);

            var crc = Crc32(Crc32(0xFFFFFFFFu, typeBytes), data) ^ 0xFFFFFFFFu;
            BinaryPrimitives.WriteUInt32BigEndian(buffer, crc);
            stream.Write(buffer);
        }

        private static uint Crc32(uint crc, byte[] data)
        {
            foreach (var b in data)
            {
                crc ^= b;
                for (var k = 0; k < 8; k++)
                {
                    crc = (crc & 1) != 0 ? 0xEDB88320u ^ (crc >> 1) : crc >> 1;
                }
            }

            return crc;
        }
    }
}
