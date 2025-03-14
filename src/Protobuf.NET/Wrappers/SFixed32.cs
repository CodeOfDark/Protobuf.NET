namespace Protobuf.NET.Wrappers;

/// <summary>
/// Represents a fixed-size 32-bit signed integer (4 bytes)
/// </summary>
public readonly struct SFixed32(int value)
{
    public readonly int Value = value;

    public static implicit operator int(SFixed32 value) => value.Value;
    public static implicit operator SFixed32(int value) => new(value);
        
    public override string ToString() => Value.ToString();
}