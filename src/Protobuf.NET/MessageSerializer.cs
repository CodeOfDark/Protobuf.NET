using System.Collections;
using System.Dynamic;
using System.Runtime.CompilerServices;
using Protobuf.Core;
using Protobuf.Core.Enums;
using Protobuf.Core.Interfaces;
using Protobuf.NET.Interfaces;
using Protobuf.NET.Wrappers;

namespace Protobuf.NET;

/// <summary>
/// Protocol Buffer serializer for dynamic objects
/// </summary>
public class MessageSerializer : IMessageSerializer
{
    /// <summary>
    /// Serializes a dynamic object to Protocol Buffer binary format
    /// </summary>
    /// <param name="message">The dynamic object to serialize</param>
    /// <returns>Serialized byte array</returns>
    public byte[] Serialize(dynamic message)
    {
        var output = new CodedOutputStream();
        SerializeObject(message, output);
        return output.ToByteArray();
    }

    /// <summary>
    /// Recursively serializes a dynamic object and writes it to the output stream
    /// </summary>
    /// <param name="obj">The dynamic object to serialize</param>
    /// <param name="output">The stream to write to</param>
    private void SerializeObject(dynamic obj, ICodedOutputStream output)
    {
        if (obj is not ExpandoObject expandoObj) 
            return;
        
        foreach (var property in expandoObj as IDictionary<string, object>)
        {
            if (property.Value is not ITuple { Length: >= 2 } tuple)
                continue;
            
            var fieldNumber = Convert.ToInt32(tuple[0]);
            dynamic fieldValue = tuple[1] ?? throw new InvalidOperationException();
            WriteField(fieldNumber, fieldValue, output);
        }
    }

    /// <summary>
    /// Writes a field to the output stream with appropriate wire format
    /// </summary>
    /// <param name="fieldNumber">The field number</param>
    /// <param name="value">The field value</param>
    /// <param name="output">The output stream</param>
    private void WriteField(int fieldNumber, dynamic value, ICodedOutputStream output)
    {
        if (value == null) 
            return;
        
        if (value is ExpandoObject nestedObj)
        {
            output.WriteTag(fieldNumber, WireFormat.LengthDelimited);
            
            var nestedOutput = new CodedOutputStream();
            SerializeObject(nestedObj, nestedOutput);
            
            var nestedBytes = nestedOutput.ToByteArray();
            output.WriteBytes(nestedBytes);
            return;
        }
        
        switch (value)
        {
            case bool boolValue:
                output.WriteTag(fieldNumber, WireFormat.Varint);
                output.WriteBool(boolValue);
                break;
                
            case int intValue:
                output.WriteTag(fieldNumber, WireFormat.Varint);
                output.WriteInt32(intValue);
                break;
                
            case long longValue:
                output.WriteTag(fieldNumber, WireFormat.Varint);
                output.WriteInt64(longValue);
                break;
                
            case uint uintValue:
                output.WriteTag(fieldNumber, WireFormat.Varint);
                output.WriteUInt32(uintValue);
                break;
                
            case ulong ulongValue:
                output.WriteTag(fieldNumber, WireFormat.Varint);
                output.WriteUInt64(ulongValue);
                break;
                
            case float floatValue:
                output.WriteTag(fieldNumber, WireFormat.Fixed32);
                output.WriteFloat(floatValue);
                break;
                
            case double doubleValue:
                output.WriteTag(fieldNumber, WireFormat.Fixed64);
                output.WriteDouble(doubleValue);
                break;
            
            case Fixed32 fixed32Value:
                output.WriteTag(fieldNumber, WireFormat.Fixed32);
                output.WriteFixed32(fixed32Value.Value);
                break;
            
            case Fixed64 fixed64Value:
                output.WriteTag(fieldNumber, WireFormat.Fixed64);
                output.WriteFixed64(fixed64Value.Value);
                break;
            
            case SFixed32 sfixed32Value:
                output.WriteTag(fieldNumber, WireFormat.Fixed32);
                output.WriteSFixed32(sfixed32Value.Value);
                break;
            
            case SFixed64 sfixed64Value:
                output.WriteTag(fieldNumber, WireFormat.Fixed64);
                output.WriteSFixed64(sfixed64Value.Value);
                break;
            
            case SInt32 sint32Value:
                output.WriteTag(fieldNumber, WireFormat.Varint);
                output.WriteSInt32(sint32Value.Value);
                break;
            
            case SInt64 sint64Value:
                output.WriteTag(fieldNumber, WireFormat.Varint);
                output.WriteSInt64(sint64Value.Value);
                break;
                
            case string stringValue:
                output.WriteTag(fieldNumber, WireFormat.LengthDelimited);
                output.WriteString(stringValue);
                break;
                
            case byte[] bytesValue:
                output.WriteTag(fieldNumber, WireFormat.LengthDelimited);
                output.WriteBytes(bytesValue);
                break;
            
            case PackedRepeatedField<int> packedInt32:
                WritePackedRepeated(fieldNumber, packedInt32, output, (stream, val) => stream.WriteInt32(val));
                break;
                    
            case PackedRepeatedField<uint> packedUInt32:
                WritePackedRepeated(fieldNumber, packedUInt32, output, (stream, val) => stream.WriteUInt32(val));
                break;
                    
            case PackedRepeatedField<long> packedInt64:
                WritePackedRepeated(fieldNumber, packedInt64, output, (stream, val) => stream.WriteInt64(val));
                break;
                    
            case PackedRepeatedField<ulong> packedUInt64:
                WritePackedRepeated(fieldNumber, packedUInt64, output, (stream, val) => stream.WriteUInt64(val));
                break;
                    
            case PackedRepeatedField<float> packedFloat:
                WritePackedRepeated(fieldNumber, packedFloat, output, (stream, val) => stream.WriteFloat(val));
                break;
                    
            case PackedRepeatedField<double> packedDouble:
                WritePackedRepeated(fieldNumber, packedDouble, output, (stream, val) => stream.WriteDouble(val));
                break;
                    
            case PackedRepeatedField<bool> packedBool:
                WritePackedRepeated(fieldNumber, packedBool, output, (stream, val) => stream.WriteBool(val));
                break;
                    
            case PackedRepeatedField<Fixed32> packedFixed32:
                WritePackedRepeated(fieldNumber, packedFixed32, output, (stream, val) => stream.WriteFixed32(val.Value));
                break;
                    
            case PackedRepeatedField<Fixed64> packedFixed64:
                WritePackedRepeated(fieldNumber, packedFixed64, output, (stream, val) => stream.WriteFixed64(val.Value));
                break;
                    
            case PackedRepeatedField<SFixed32> packedSFixed32:
                WritePackedRepeated(fieldNumber, packedSFixed32, output, (stream, val) => stream.WriteSFixed32(val.Value));
                break;
                    
            case PackedRepeatedField<SFixed64> packedSFixed64:
                WritePackedRepeated(fieldNumber, packedSFixed64, output, (stream, val) => stream.WriteSFixed64(val.Value));
                break;
                    
            case PackedRepeatedField<SInt32> packedSInt32:
                WritePackedRepeated(fieldNumber, packedSInt32, output, (stream, val) => stream.WriteSInt32(val.Value));
                break;
                    
            case PackedRepeatedField<SInt64> packedSInt64:
                WritePackedRepeated(fieldNumber, packedSInt64, output, (stream, val) => stream.WriteSInt64(val.Value));
                break;
            
            case IEnumerable enumerable:
                WriteRepeatedField(fieldNumber, enumerable, output);
                break;
            
            case ITuple { Length: >= 2 } nestedTuple:
                output.WriteTag(fieldNumber, WireFormat.LengthDelimited);
                var innerOutput = new CodedOutputStream();
                WriteField(Convert.ToInt32(nestedTuple[0]), nestedTuple[1] ?? throw new InvalidOperationException(), innerOutput);
                var innerBytes = innerOutput.ToByteArray();
                output.WriteBytes(innerBytes);
                break;
                
            default:
                throw new ArgumentException($"Unsupported type for protobuf serialization: {value.GetType().Name}");
        }
    }

    /// <summary>
    /// Writes a repeated field to the output stream (non-packed format)
    /// </summary>
    /// <param name="fieldNumber">The field number</param>
    /// <param name="values">The collection of values</param>
    /// <param name="output">The output stream</param>
    private void WriteRepeatedField(int fieldNumber, IEnumerable values, ICodedOutputStream output)
    {
        foreach (var item in values)
            WriteField(fieldNumber, item, output);
    }
        
    /// <summary>
    /// Writes a packed repeated field to the output stream
    /// </summary>
    /// <typeparam name="T">The type of elements in the collection</typeparam>
    /// <param name="fieldNumber">The field number</param>
    /// <param name="values">The collection of values</param>
    /// <param name="output">The output stream</param>
    /// <param name="writeAction">The action to write a single value</param>
    private void WritePackedRepeated<T>(
        int fieldNumber, 
        IEnumerable<T> values, 
        ICodedOutputStream output,
        Action<ICodedOutputStream, T> writeAction)
    {
        var tempStream = new CodedOutputStream();
        foreach (var value in values)
            writeAction(tempStream, value);
        
        var packedBytes = tempStream.ToByteArray();
        
        output.WriteTag(fieldNumber, WireFormat.LengthDelimited);
        output.WriteBytes(packedBytes);
    }
}