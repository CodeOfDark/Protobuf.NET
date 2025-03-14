using Protobuf.Core.Enums;

namespace Protobuf.Core;

/// <summary>
/// Utility methods for wire format operations
/// </summary>
public static class WireFormatUtility
{
    /// <summary>
    /// Gets the wire type for a field type
    /// </summary>
    public static WireFormat GetWireType(FieldType fieldType)
    {
        return fieldType switch
        {
            FieldType.Int32 or FieldType.Int64 or FieldType.UInt32 or FieldType.UInt64 or FieldType.SInt32 or FieldType.SInt64 or FieldType.Bool or FieldType.Enum 
                => WireFormat.Varint,
            FieldType.Fixed64 or FieldType.SFixed64 or FieldType.Double 
                => WireFormat.Fixed64,
            FieldType.String or FieldType.Message or FieldType.Bytes 
                => WireFormat.LengthDelimited,
            FieldType.Fixed32 or FieldType.SFixed32 or FieldType.Float 
                => WireFormat.Fixed32,
            _ => throw new ArgumentException($"Unsupported field type: {fieldType}")
        };
    }
        
    /// <summary>
    /// Creates a tag value from field number and wire type
    /// </summary>
    public static uint MakeTag(int fieldNumber, WireFormat wireType)
    {
        return (uint)((fieldNumber << 3) | (int)wireType);
    }
        
    /// <summary>
    /// Gets the field number from a tag
    /// </summary>
    public static int GetFieldNumberFromTag(uint tag)
    {
        return (int)(tag >> 3);
    }
        
    /// <summary>
    /// Gets the wire type from a tag
    /// </summary>
    public static WireFormat GetWireTypeFromTag(uint tag)
    {
        return (WireFormat)(tag & 0x7);
    }
    
    /// <summary>
    /// Gets the appropriate wire format for a given .NET type
    /// </summary>
    /// <param name="type">The .NET type</param>
    /// <returns>The appropriate wire format</returns>
    public static WireFormat GetWireFormatForType(Type type)
    {
        if (type == typeof(bool) || type == typeof(int) || type == typeof(long) || 
            type == typeof(uint) || type == typeof(ulong) || type == typeof(byte) || 
            type == typeof(sbyte) || type == typeof(short) || type == typeof(ushort))
            return WireFormat.Varint;

        if (type == typeof(float) || type == typeof(int))
            return WireFormat.Fixed32;

        if (type == typeof(double) || type == typeof(long))
            return WireFormat.Fixed64;

        if (type == typeof(string) || type == typeof(byte[]) || type.IsClass)
            return WireFormat.LengthDelimited;

        throw new ArgumentException($"No appropriate wire format for type: {type.Name}");
    }
}