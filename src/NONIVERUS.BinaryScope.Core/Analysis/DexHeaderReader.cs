using System.Buffers.Binary;
using NONIVERUS.BinaryScope.Models;

namespace NONIVERUS.BinaryScope.Analysis;

public static class DexHeaderReader
{
    private const int DexHeaderMinimum = 112;

    public static DexHeaderMetrics Read(Stream stream)
    {
        var header = new byte[DexHeaderMinimum];
        var total = 0;
        while (total < header.Length)
        {
            var read = stream.Read(header, total, header.Length - total);
            if (read == 0) break;
            total += read;
        }

        if (total < DexHeaderMinimum)
            return Invalid("DEX header is shorter than 112 bytes.");

        if (header[0] != (byte)'d' || header[1] != (byte)'e' || header[2] != (byte)'x' || header[3] != (byte)'\n')
            return Invalid("DEX magic was not found.");

        var version = System.Text.Encoding.ASCII.GetString(header, 4, 3);
        if (header[7] != 0)
            return Invalid("DEX magic terminator is invalid.", version);

        var endian = BinaryPrimitives.ReadUInt32LittleEndian(header.AsSpan(40, 4));
        if (endian != 0x12345678)
            return Invalid($"Unsupported DEX endian tag 0x{endian:X8}.", version);

        var declaredFileSize = U32(header, 32);
        var headerSize = U32(header, 36);
        var warning = headerSize == DexHeaderMinimum ? null : $"Unexpected DEX header size: {headerSize}.";

        return new DexHeaderMetrics(
            version,
            declaredFileSize,
            headerSize,
            U32(header, 56),
            U32(header, 64),
            U32(header, 72),
            U32(header, 80),
            U32(header, 88),
            U32(header, 96),
            true,
            warning);
    }

    private static uint U32(byte[] bytes, int offset) =>
        BinaryPrimitives.ReadUInt32LittleEndian(bytes.AsSpan(offset, 4));

    private static DexHeaderMetrics Invalid(string warning, string version = "unknown") =>
        new(version, 0, 0, 0, 0, 0, 0, 0, 0, false, warning);
}
