namespace Protobuf.NET.Wrappers;

/// <summary>
/// Represents a 64-bit signed integer (varint)
/// </summary>
public readonly struct SInt64(long value)
{
    public readonly long Value = value;

    public static implicit operator long(SInt64 value) => value.Value;
    public static implicit operator SInt64(long value) => new(value);
        
    public override string ToString() => Value.ToString();
}