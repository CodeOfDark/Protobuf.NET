namespace Protobuf.NET.Wrappers;

/// <summary>
/// Represents a fixed-size 32-bit unsigned integer (4 bytes)
/// </summary>
public readonly struct Fixed32(uint value)
{
    public readonly uint Value = value;

    public static implicit operator uint(Fixed32 value) => value.Value;
    public static implicit operator Fixed32(uint value) => new(value);
    
    public override string ToString() => Value.ToString();
}