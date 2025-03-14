namespace Protobuf.NET.Wrappers;

/// <summary>
/// Represents a 32-bit signed integer (varint)
/// </summary>
public readonly struct SInt32(int value)
{
    public readonly int Value = value;

    public static implicit operator int(SInt32 value) => value.Value;
    public static implicit operator SInt32(int value) => new(value);
        
    public override string ToString() => Value.ToString();
}