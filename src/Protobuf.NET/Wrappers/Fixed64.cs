namespace Protobuf.NET.Wrappers;

/// <summary>
/// Represents a fixed-size 64-bit unsigned integer (8 bytes)
/// </summary>
public readonly struct Fixed64(ulong value)
{
    public readonly ulong Value = value;

    public static implicit operator ulong(Fixed64 value) => value.Value;
    public static implicit operator Fixed64(ulong value) => new(value);
        
    public override string ToString() => Value.ToString();
}