using System.Dynamic;
using Protobuf.NET.Wrappers;

namespace Protobuf.NET;

/// <summary>
/// Fluent builder for Protocol Buffer messages using ExpandoObject
/// </summary>
public class ProtobufMessageBuilder
{
    private readonly ExpandoObject _message;
    private readonly IDictionary<string, object> _dictionary;

    /// <summary>
    /// Initializes a new instance of the ProtobufMessageBuilder class
    /// </summary>
    public ProtobufMessageBuilder()
    {
        _message = new ExpandoObject();
        _dictionary = _message as IDictionary<string, object>;
    }

    /// <summary>
    /// Adds a field to the message
    /// </summary>
    /// <typeparam name="T">Type of the field value</typeparam>
    /// <param name="name">The field name</param>
    /// <param name="fieldNumber">The field number</param>
    /// <param name="value">The field value</param>
    /// <returns>The builder for method chaining</returns>
    public ProtobufMessageBuilder AddField<T>(string name, int fieldNumber, T value)
    {
        _dictionary[name] = (fieldNumber, value);
        return this;
    }

    /// <summary>
    /// Builds the message
    /// </summary>
    /// <returns>The built ExpandoObject message</returns>
    public dynamic Build()
    {
        return _message;
    }

    /// <summary>
    /// Builds and serializes the message
    /// </summary>
    /// <returns>Serialized byte array</returns>
    public byte[] BuildAndSerialize()
    {
        return ProtobufSerializer.Serialize(_message);
    }
    
    /// <summary>
    /// Adds a fixed32 field to the message
    /// </summary>
    public ProtobufMessageBuilder AddFixed32Field(string name, int fieldNumber, uint value)
    {
        return AddField(name, fieldNumber, ProtobufTypes.AsFixed32(value));
    }
        
    /// <summary>
    /// Adds a fixed64 field to the message
    /// </summary>
    public ProtobufMessageBuilder AddFixed64Field(string name, int fieldNumber, ulong value)
    {
        return AddField(name, fieldNumber, ProtobufTypes.AsFixed64(value));
    }
        
    /// <summary>
    /// Adds a sfixed32 field to the message
    /// </summary>
    public ProtobufMessageBuilder AddSFixed32Field(string name, int fieldNumber, int value)
    {
        return AddField(name, fieldNumber, ProtobufTypes.AsSFixed32(value));
    }
        
    /// <summary>
    /// Adds a sfixed64 field to the message
    /// </summary>
    public ProtobufMessageBuilder AddSFixed64Field(string name, int fieldNumber, long value)
    {
        return AddField(name, fieldNumber, ProtobufTypes.AsSFixed64(value));
    }
        
    /// <summary>
    /// Adds a sint32 field to the message
    /// </summary>
    public ProtobufMessageBuilder AddSInt32Field(string name, int fieldNumber, int value)
    {
        return AddField(name, fieldNumber, ProtobufTypes.AsSInt32(value));
    }
        
    /// <summary>
    /// Adds a sint64 field to the message
    /// </summary>
    public ProtobufMessageBuilder AddSInt64Field(string name, int fieldNumber, long value)
    {
        return AddField(name, fieldNumber, ProtobufTypes.AsSInt64(value));
    }
    
    /// <summary>
    /// Adds a packed repeated int32 field to the message
    /// </summary>
    public ProtobufMessageBuilder AddPackedInt32Field(string name, int fieldNumber, IEnumerable<int> values)
    {
        return AddField(name, fieldNumber, ProtobufTypes.PackedInt32(values));
    }
        
    /// <summary>
    /// Adds a packed repeated uint32 field to the message
    /// </summary>
    public ProtobufMessageBuilder AddPackedUInt32Field(string name, int fieldNumber, IEnumerable<uint> values)
    {
        return AddField(name, fieldNumber, ProtobufTypes.PackedUInt32(values));
    }
        
    /// <summary>
    /// Adds a packed repeated int64 field to the message
    /// </summary>
    public ProtobufMessageBuilder AddPackedInt64Field(string name, int fieldNumber, IEnumerable<long> values)
    {
        return AddField(name, fieldNumber, ProtobufTypes.PackedInt64(values));
    }
        
    /// <summary>
    /// Adds a packed repeated uint64 field to the message
    /// </summary>
    public ProtobufMessageBuilder AddPackedUInt64Field(string name, int fieldNumber, IEnumerable<ulong> values)
    {
        return AddField(name, fieldNumber, ProtobufTypes.PackedUInt64(values));
    }
        
    /// <summary>
    /// Adds a packed repeated float field to the message
    /// </summary>
    public ProtobufMessageBuilder AddPackedFloatField(string name, int fieldNumber, IEnumerable<float> values)
    {
        return AddField(name, fieldNumber, ProtobufTypes.PackedFloat(values));
    }
        
    /// <summary>
    /// Adds a packed repeated double field to the message
    /// </summary>
    public ProtobufMessageBuilder AddPackedDoubleField(string name, int fieldNumber, IEnumerable<double> values)
    {
        return AddField(name, fieldNumber, ProtobufTypes.PackedDouble(values));
    }
        
    /// <summary>
    /// Adds a packed repeated bool field to the message
    /// </summary>
    public ProtobufMessageBuilder AddPackedBoolField(string name, int fieldNumber, IEnumerable<bool> values)
    {
        return AddField(name, fieldNumber, ProtobufTypes.PackedBool(values));
    }
        
    /// <summary>
    /// Adds a packed repeated fixed32 field to the message
    /// </summary>
    public ProtobufMessageBuilder AddPackedFixed32Field(string name, int fieldNumber, IEnumerable<uint> values)
    {
        return AddField(name, fieldNumber, ProtobufTypes.PackedFixed32(values));
    }
        
    /// <summary>
    /// Adds a packed repeated fixed64 field to the message
    /// </summary>
    public ProtobufMessageBuilder AddPackedFixed64Field(string name, int fieldNumber, IEnumerable<ulong> values)
    {
        return AddField(name, fieldNumber, ProtobufTypes.PackedFixed64(values));
    }
        
    /// <summary>
    /// Adds a packed repeated sfixed32 field to the message
    /// </summary>
    public ProtobufMessageBuilder AddPackedSFixed32Field(string name, int fieldNumber, IEnumerable<int> values)
    {
        return AddField(name, fieldNumber, ProtobufTypes.PackedSFixed32(values));
    }
        
    /// <summary>
    /// Adds a packed repeated sfixed64 field to the message
    /// </summary>
    public ProtobufMessageBuilder AddPackedSFixed64Field(string name, int fieldNumber, IEnumerable<long> values)
    {
        return AddField(name, fieldNumber, ProtobufTypes.PackedSFixed64(values));
    }
        
    /// <summary>
    /// Adds a packed repeated sint32 field to the message
    /// </summary>
    public ProtobufMessageBuilder AddPackedSInt32Field(string name, int fieldNumber, IEnumerable<int> values)
    {
        return AddField(name, fieldNumber, ProtobufTypes.PackedSInt32(values));
    }
        
    /// <summary>
    /// Adds a packed repeated sint64 field to the message
    /// </summary>
    public ProtobufMessageBuilder AddPackedSInt64Field(string name, int fieldNumber, IEnumerable<long> values)
    {
        return AddField(name, fieldNumber, ProtobufTypes.PackedSInt64(values));
    }
}