namespace Protobuf.NET.Wrappers;

/// <summary>
/// Static factory for creating Protocol Buffer type wrappers
/// </summary>
public static class ProtobufTypes
{
    public static Fixed32 AsFixed32(uint value) => new(value);
    public static Fixed64 AsFixed64(ulong value) => new(value);
    public static SFixed32 AsSFixed32(int value) => new(value);
    public static SFixed64 AsSFixed64(long value) => new(value);
    public static SInt32 AsSInt32(int value) => new(value);
    public static SInt64 AsSInt64(long value) => new(value);
    
    public static PackedInt32 PackedInt32(IEnumerable<int> values) => new(values);
    public static PackedUInt32 PackedUInt32(IEnumerable<uint> values) => new(values);
    public static PackedInt64 PackedInt64(IEnumerable<long> values) => new(values);
    public static PackedUInt64 PackedUInt64(IEnumerable<ulong> values) => new(values);
    public static PackedFloat PackedFloat(IEnumerable<float> values) => new(values);
    public static PackedDouble PackedDouble(IEnumerable<double> values) => new(values);
    public static PackedBool PackedBool(IEnumerable<bool> values) => new(values);
    public static PackedFixed32 PackedFixed32(IEnumerable<uint> values) => new(values);
    public static PackedFixed64 PackedFixed64(IEnumerable<ulong> values) => new(values);
    public static PackedSFixed32 PackedSFixed32(IEnumerable<int> values) => new(values);
    public static PackedSFixed64 PackedSFixed64(IEnumerable<long> values) => new(values);
    public static PackedSInt32 PackedSInt32(IEnumerable<int> values) => new(values);
    public static PackedSInt64 PackedSInt64(IEnumerable<long> values) => new(values);
}