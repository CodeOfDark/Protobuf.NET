namespace Protobuf.NET.Wrappers;

/// <summary>
/// Represents a fixed-size 64-bit signed integer (8 bytes)
/// </summary>
public readonly struct SFixed64(long value)
{
    public readonly long Value = value;

    public static implicit operator long(SFixed64 value) => value.Value;
    public static implicit operator SFixed64(long value) => new(value);
        
    public override string ToString() => Value.ToString();
}