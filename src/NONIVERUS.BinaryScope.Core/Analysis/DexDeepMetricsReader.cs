using System.Buffers.Binary;
using System.Text;

namespace NONIVERUS.BinaryScope.Analysis;

internal sealed record DexDeepMetrics(
    ulong DefinedMethods,
    ulong MethodsWithCode,
    IReadOnlyCollection<string> MethodReferences);

internal static class DexDeepMetricsReader
{
    private const int HeaderSize = 112;

    public static DexDeepMetrics Read(byte[] bytes)
    {
        ArgumentNullException.ThrowIfNull(bytes);
        if (bytes.Length < HeaderSize)
            throw new InvalidDataException("DEX is shorter than the minimum header size.");

        var reader = new Reader(bytes);
        reader.ValidateDex();

        var methodReferences = reader.ReadMethodReferences();
        var (definedMethods, methodsWithCode) = reader.ReadDefinedMethodMetrics();

        return new DexDeepMetrics(definedMethods, methodsWithCode, methodReferences);
    }

    private sealed class Reader
    {
        private readonly byte[] _bytes;
        private readonly uint _stringIdsSize;
        private readonly uint _stringIdsOff;
        private readonly uint _typeIdsSize;
        private readonly uint _typeIdsOff;
        private readonly uint _protoIdsSize;
        private readonly uint _protoIdsOff;
        private readonly uint _methodIdsSize;
        private readonly uint _methodIdsOff;
        private readonly uint _classDefsSize;
        private readonly uint _classDefsOff;
        private readonly string?[] _stringCache;
        private readonly string?[] _typeCache;
        private readonly string?[] _protoCache;

        public Reader(byte[] bytes)
        {
            _bytes = bytes;
            _stringIdsSize = U32(56);
            _stringIdsOff = U32(60);
            _typeIdsSize = U32(64);
            _typeIdsOff = U32(68);
            _protoIdsSize = U32(72);
            _protoIdsOff = U32(76);
            _methodIdsSize = U32(88);
            _methodIdsOff = U32(92);
            _classDefsSize = U32(96);
            _classDefsOff = U32(100);
            _stringCache = new string?[CheckedCount(_stringIdsSize, "string_ids")];
            _typeCache = new string?[CheckedCount(_typeIdsSize, "type_ids")];
            _protoCache = new string?[CheckedCount(_protoIdsSize, "proto_ids")];
        }

        public void ValidateDex()
        {
            if (_bytes[0] != (byte)'d' || _bytes[1] != (byte)'e' || _bytes[2] != (byte)'x' || _bytes[3] != (byte)'\n')
                throw new InvalidDataException("DEX magic was not found.");
            if (_bytes[7] != 0)
                throw new InvalidDataException("DEX magic terminator is invalid.");

            var endian = U32(40);
            if (endian != 0x12345678)
                throw new InvalidDataException($"Unsupported DEX endian tag 0x{endian:X8}.");

            ValidateTable(_stringIdsOff, _stringIdsSize, 4, "string_ids");
            ValidateTable(_typeIdsOff, _typeIdsSize, 4, "type_ids");
            ValidateTable(_protoIdsOff, _protoIdsSize, 12, "proto_ids");
            ValidateTable(_methodIdsOff, _methodIdsSize, 8, "method_ids");
            ValidateTable(_classDefsOff, _classDefsSize, 32, "class_defs");
        }

        public IReadOnlyCollection<string> ReadMethodReferences()
        {
            var result = new HashSet<string>(StringComparer.Ordinal);
            for (uint i = 0; i < _methodIdsSize; i++)
            {
                var offset = TableOffset(_methodIdsOff, i, 8, "method_id");
                var classIdx = U16(offset);
                var protoIdx = U16(offset + 2);
                var nameIdx = U32(offset + 4);

                var declaringType = TypeDescriptor(classIdx);
                var name = StringValue(nameIdx);
                var prototype = ProtoSignature(protoIdx);
                result.Add($"{declaringType}->{name}{prototype}");
            }
            return result;
        }

        public (ulong DefinedMethods, ulong MethodsWithCode) ReadDefinedMethodMetrics()
        {
            ulong defined = 0;
            ulong withCode = 0;

            for (uint i = 0; i < _classDefsSize; i++)
            {
                var classDef = TableOffset(_classDefsOff, i, 32, "class_def");
                var classDataOff = U32(classDef + 24);
                if (classDataOff == 0)
                    continue;

                var cursor = CheckedOffset(classDataOff, "class_data_off");
                var staticFields = ReadUleb128(ref cursor);
                var instanceFields = ReadUleb128(ref cursor);
                var directMethods = ReadUleb128(ref cursor);
                var virtualMethods = ReadUleb128(ref cursor);

                SkipEncodedFields(ref cursor, staticFields);
                SkipEncodedFields(ref cursor, instanceFields);
                ReadEncodedMethods(ref cursor, directMethods, ref defined, ref withCode);
                ReadEncodedMethods(ref cursor, virtualMethods, ref defined, ref withCode);
            }

            return (defined, withCode);
        }

        private void SkipEncodedFields(ref int cursor, uint count)
        {
            for (uint i = 0; i < count; i++)
            {
                _ = ReadUleb128(ref cursor); // field_idx_diff
                _ = ReadUleb128(ref cursor); // access_flags
            }
        }

        private void ReadEncodedMethods(ref int cursor, uint count, ref ulong defined, ref ulong withCode)
        {
            uint methodIndex = 0;
            for (uint i = 0; i < count; i++)
            {
                var diff = ReadUleb128(ref cursor);
                methodIndex = checked(methodIndex + diff);
                if (methodIndex >= _methodIdsSize)
                    throw new InvalidDataException("Encoded method index exceeds method_ids_size.");

                _ = ReadUleb128(ref cursor); // access_flags
                var codeOff = ReadUleb128(ref cursor);
                defined++;
                if (codeOff != 0)
                    withCode++;
            }
        }

        private string StringValue(uint index)
        {
            if (index >= _stringIdsSize)
                throw new InvalidDataException($"String index {index} exceeds string_ids_size {_stringIdsSize}.");

            var cacheIndex = checked((int)index);
            var cached = _stringCache[cacheIndex];
            if (cached is not null)
                return cached;

            var itemOffset = TableOffset(_stringIdsOff, index, 4, "string_id");
            var dataOff = U32(itemOffset);
            var cursor = CheckedOffset(dataOff, "string_data_off");
            _ = ReadUleb128(ref cursor); // UTF-16 code-unit count. Content ends at NUL.

            var builder = new StringBuilder();
            while (true)
            {
                Ensure(cursor, 1, "string_data_item");
                var b0 = _bytes[cursor++];
                if (b0 == 0)
                    break;

                if ((b0 & 0x80) == 0)
                {
                    builder.Append((char)b0);
                    continue;
                }

                if ((b0 & 0xE0) == 0xC0)
                {
                    Ensure(cursor, 1, "MUTF-8 two-byte sequence");
                    var b1 = _bytes[cursor++];
                    RequireContinuation(b1);
                    var value = ((b0 & 0x1F) << 6) | (b1 & 0x3F);
                    builder.Append((char)value);
                    continue;
                }

                if ((b0 & 0xF0) == 0xE0)
                {
                    Ensure(cursor, 2, "MUTF-8 three-byte sequence");
                    var b1 = _bytes[cursor++];
                    var b2 = _bytes[cursor++];
                    RequireContinuation(b1);
                    RequireContinuation(b2);
                    var value = ((b0 & 0x0F) << 12) | ((b1 & 0x3F) << 6) | (b2 & 0x3F);
                    builder.Append((char)value);
                    continue;
                }

                throw new InvalidDataException("Unsupported byte sequence in DEX MUTF-8 string.");
            }

            var valueString = builder.ToString();
            _stringCache[cacheIndex] = valueString;
            return valueString;
        }

        private string TypeDescriptor(uint index)
        {
            if (index >= _typeIdsSize)
                throw new InvalidDataException($"Type index {index} exceeds type_ids_size {_typeIdsSize}.");

            var cacheIndex = checked((int)index);
            var cached = _typeCache[cacheIndex];
            if (cached is not null)
                return cached;

            var itemOffset = TableOffset(_typeIdsOff, index, 4, "type_id");
            var descriptorIdx = U32(itemOffset);
            var descriptor = StringValue(descriptorIdx);
            _typeCache[cacheIndex] = descriptor;
            return descriptor;
        }

        private string ProtoSignature(uint index)
        {
            if (index >= _protoIdsSize)
                throw new InvalidDataException($"Proto index {index} exceeds proto_ids_size {_protoIdsSize}.");

            var cacheIndex = checked((int)index);
            var cached = _protoCache[cacheIndex];
            if (cached is not null)
                return cached;

            var itemOffset = TableOffset(_protoIdsOff, index, 12, "proto_id");
            var returnTypeIdx = U32(itemOffset + 4);
            var parametersOff = U32(itemOffset + 8);

            var builder = new StringBuilder("(");
            if (parametersOff != 0)
            {
                var cursor = CheckedOffset(parametersOff, "parameters_off");
                var size = U32(cursor);
                cursor += 4;
                for (uint i = 0; i < size; i++)
                {
                    Ensure(cursor, 2, "type_list");
                    var typeIdx = U16(cursor);
                    cursor += 2;
                    builder.Append(TypeDescriptor(typeIdx));
                }
            }

            builder.Append(')');
            builder.Append(TypeDescriptor(returnTypeIdx));
            var signature = builder.ToString();
            _protoCache[cacheIndex] = signature;
            return signature;
        }

        private uint ReadUleb128(ref int cursor)
        {
            uint result = 0;
            var shift = 0;
            for (var i = 0; i < 5; i++)
            {
                Ensure(cursor, 1, "uleb128");
                var b = _bytes[cursor++];
                result |= (uint)(b & 0x7F) << shift;
                if ((b & 0x80) == 0)
                    return result;
                shift += 7;
            }
            throw new InvalidDataException("ULEB128 value exceeds 5 bytes.");
        }

        private ushort U16(int offset)
        {
            Ensure(offset, 2, "uint16");
            return BinaryPrimitives.ReadUInt16LittleEndian(_bytes.AsSpan(offset, 2));
        }

        private uint U32(int offset)
        {
            Ensure(offset, 4, "uint32");
            return BinaryPrimitives.ReadUInt32LittleEndian(_bytes.AsSpan(offset, 4));
        }

        private int TableOffset(uint tableOff, uint index, int itemSize, string tableName)
        {
            var offset = (ulong)tableOff + ((ulong)index * (uint)itemSize);
            if (offset > int.MaxValue)
                throw new InvalidDataException($"{tableName} offset exceeds supported range.");
            var result = (int)offset;
            Ensure(result, itemSize, tableName);
            return result;
        }

        private int CheckedOffset(uint offset, string name)
        {
            if (offset > int.MaxValue)
                throw new InvalidDataException($"{name} exceeds supported range.");
            var result = (int)offset;
            Ensure(result, 1, name);
            return result;
        }

        private void ValidateTable(uint offset, uint count, int itemSize, string name)
        {
            if (count == 0)
                return;
            var end = (ulong)offset + ((ulong)count * (uint)itemSize);
            if (offset == 0 || end > (ulong)_bytes.Length)
                throw new InvalidDataException($"DEX {name} table exceeds file bounds.");
        }

        private void Ensure(int offset, int size, string name)
        {
            if (offset < 0 || size < 0 || offset > _bytes.Length - size)
                throw new InvalidDataException($"DEX {name} read exceeds file bounds.");
        }

        private static int CheckedCount(uint value, string name)
        {
            if (value > int.MaxValue)
                throw new InvalidDataException($"DEX {name} count exceeds supported range.");
            return (int)value;
        }

        private static void RequireContinuation(byte value)
        {
            if ((value & 0xC0) != 0x80)
                throw new InvalidDataException("Invalid DEX MUTF-8 continuation byte.");
        }
    }
}
