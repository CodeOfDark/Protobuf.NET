using System.Dynamic;
using Protobuf.NET.Wrappers;

namespace Protobuf.NET.Extensions;

/// <summary>
/// Extension methods for working with ExpandoObject in Protocol Buffer serialization
/// </summary>
public static class ExpandoObjectExtensions
{
    /// <summary>
    /// Serializes an ExpandoObject to Protocol Buffer binary format
    /// </summary>
    /// <param name="obj">The ExpandoObject to serialize</param>
    /// <returns>Serialized byte array</returns>
    public static byte[] ToProtobuf(this ExpandoObject obj)
    {
        return ProtobufSerializer.Serialize(obj);
    }

    /// <summary>
    /// Helper method to create a Protocol Buffer field tuple
    /// </summary>
    /// <typeparam name="T">Type of the field value</typeparam>
    /// <param name="fieldNumber">The field number</param>
    /// <param name="value">The field value</param>
    /// <returns>A tuple representing the field</returns>
    public static (int, T) Field<T>(int fieldNumber, T value)
    {
        return (fieldNumber, value);
    }
        
    /// <summary>
    /// Creates a Fixed32 field value
    /// </summary>
    public static Fixed32 Fixed32(uint value)
    {
        return ProtobufTypes.AsFixed32(value);
    }
        
    /// <summary>
    /// Creates a Fixed64 field value
    /// </summary>
    public static Fixed64 Fixed64(ulong value)
    {
        return ProtobufTypes.AsFixed64(value);
    }
        
    /// <summary>
    /// Creates a SFixed32 field value
    /// </summary>
    public static SFixed32 SFixed32(int value)
    {
        return ProtobufTypes.AsSFixed32(value);
    }
        
    /// <summary>
    /// Creates a SFixed64 field value
    /// </summary>
    public static SFixed64 SFixed64(long value)
    {
        return ProtobufTypes.AsSFixed64(value);
    }
        
    /// <summary>
    /// Creates a SInt32 field value
    /// </summary>
    public static SInt32 SInt32(int value)
    {
        return ProtobufTypes.AsSInt32(value);
    }
        
    /// <summary>
    /// Creates a SInt64 field value
    /// </summary>
    public static SInt64 SInt64(long value)
    {
        return ProtobufTypes.AsSInt64(value);
    }
    
    /// <summary>
    /// Creates a packed repeated int32 field
    /// </summary>
    public static PackedInt32 PackedInt32(IEnumerable<int> values)
    {
        return ProtobufTypes.PackedInt32(values);
    }
        
    /// <summary>
    /// Creates a packed repeated uint32 field
    /// </summary>
    public static PackedUInt32 PackedUInt32(IEnumerable<uint> values)
    {
        return ProtobufTypes.PackedUInt32(values);
    }
        
    /// <summary>
    /// Creates a packed repeated int64 field
    /// </summary>
    public static PackedInt64 PackedInt64(IEnumerable<long> values)
    {
        return ProtobufTypes.PackedInt64(values);
    }
        
    /// <summary>
    /// Creates a packed repeated uint64 field
    /// </summary>
    public static PackedUInt64 PackedUInt64(IEnumerable<ulong> values)
    {
        return ProtobufTypes.PackedUInt64(values);
    }
        
    /// <summary>
    /// Creates a packed repeated float field
    /// </summary>
    public static PackedFloat PackedFloat(IEnumerable<float> values)
    {
        return ProtobufTypes.PackedFloat(values);
    }
        
    /// <summary>
    /// Creates a packed repeated double field
    /// </summary>
    public static PackedDouble PackedDouble(IEnumerable<double> values)
    {
        return ProtobufTypes.PackedDouble(values);
    }
        
    /// <summary>
    /// Creates a packed repeated bool field
    /// </summary>
    public static PackedBool PackedBool(IEnumerable<bool> values)
    {
        return ProtobufTypes.PackedBool(values);
    }
        
    /// <summary>
    /// Creates a packed repeated fixed32 field
    /// </summary>
    public static PackedFixed32 PackedFixed32(IEnumerable<uint> values)
    {
        return ProtobufTypes.PackedFixed32(values);
    }
        
    /// <summary>
    /// Creates a packed repeated fixed64 field
    /// </summary>
    public static PackedFixed64 PackedFixed64(IEnumerable<ulong> values)
    {
        return ProtobufTypes.PackedFixed64(values);
    }
        
    /// <summary>
    /// Creates a packed repeated sfixed32 field
    /// </summary>
    public static PackedSFixed32 PackedSFixed32(IEnumerable<int> values)
    {
        return ProtobufTypes.PackedSFixed32(values);
    }
        
    /// <summary>
    /// Creates a packed repeated sfixed64 field
    /// </summary>
    public static PackedSFixed64 PackedSFixed64(IEnumerable<long> values)
    {
        return ProtobufTypes.PackedSFixed64(values);
    }
        
    /// <summary>
    /// Creates a packed repeated sint32 field
    /// </summary>
    public static PackedSInt32 PackedSInt32(IEnumerable<int> values)
    {
        return ProtobufTypes.PackedSInt32(values);
    }
        
    /// <summary>
    /// Creates a packed repeated sint64 field
    /// </summary>
    public static PackedSInt64 PackedSInt64(IEnumerable<long> values)
    {
        return ProtobufTypes.PackedSInt64(values);
    }

    /// <summary>
    /// Creates a new dynamic object builder for Protocol Buffer messages
    /// </summary>
    /// <returns>A new dynamic object builder</returns>
    public static ProtobufMessageBuilder CreateMessage()
    {
        return new ProtobufMessageBuilder();
    }
}