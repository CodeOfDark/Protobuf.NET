using Protobuf.Core;
using Protobuf.Core.Enums;
using Protobuf.Core.Interfaces;
using Protobuf.NET.Exceptions;
using Protobuf.NET.Interfaces;

namespace Protobuf.NET;

/// <summary>
/// A deserializer that produces dynamic object-based Protocol Buffers results.
/// </summary>
public class DynamicDeserializer : IDeserializer<DynamicProtobufResult>
{
    private readonly ICodedInputStream _input;
    private readonly DeserializerOptions _options;
    private int _currentNestingDepth;
    
    /// <summary>
    /// Initializes a new instance of the <see cref="DynamicDeserializer"/> class.
    /// </summary>
    /// <param name="input">The input stream containing Protocol Buffers data.</param>
    /// <param name="options">Options to configure the deserialization process.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="input"/> is null.</exception>
    public DynamicDeserializer(ICodedInputStream input, DeserializerOptions? options = null)
    {
        _input = input ?? throw new ArgumentNullException(nameof(input));
        _options = options ?? new DeserializerOptions();
        _currentNestingDepth = 0;
    }

    /// <summary>
    /// Deserializes Protocol Buffers data into a dynamic object-based result.
    /// </summary>
    /// <param name="input">Optional secondary input stream. If null, the main input stream is used.</param>
    /// <returns>A dynamic object-based result containing the deserialized data.</returns>
    /// <exception cref="ProtobufDeserializationException">
    /// Thrown when deserialization fails and <see cref="DeserializerOptions.ThrowOnError"/> is true.
    /// </exception>
    /// <exception cref="ProtobufMaxNestingDepthExceededException">
    /// Thrown when the maximum nesting depth is exceeded.
    /// </exception>
    public DynamicProtobufResult Deserialize(ICodedInputStream? input = null)
    {
        if (_currentNestingDepth > _options.MaxNestingDepth)
        {
            var exception = new ProtobufMaxNestingDepthExceededException($"Maximum nesting depth of {_options.MaxNestingDepth} exceeded.");
            if (_options.ThrowOnError)
                throw exception;
                
            return new DynamicProtobufResult();
        }
        
        _currentNestingDepth++;
        try
        {
            var activeInput = input ?? _input;
            
            var result = new DynamicProtobufResult();
            while (!activeInput.IsAtEnd())
            {
                var tag = activeInput.ReadTag();
                var wireType = WireFormatUtility.GetWireTypeFromTag(tag);
                var fieldNumber = WireFormatUtility.GetFieldNumberFromTag(tag);

                DeserializeByWireType(activeInput, wireType, fieldNumber, result);
            }
            
            return result;
        }
        catch (Exception) when (!_options.ThrowOnError)
        {
            return new DynamicProtobufResult();
        }
        finally
        {
            _currentNestingDepth--;
        }
    }
    
    private void DeserializeByWireType(ICodedInputStream input, WireFormat wireType, int fieldNumber, DynamicProtobufResult result)
    {
        try
        {
            switch (wireType)
            {
                case WireFormat.Varint:
                    DeserializeVarint(input, fieldNumber, result);
                    break;
                case WireFormat.Fixed32:
                    DeserializeFixed32(input, fieldNumber, result);
                    break;
                case WireFormat.Fixed64:
                    DeserializeFixed64(input, fieldNumber, result);
                    break;
                case WireFormat.LengthDelimited:
                    DeserializeLengthDelimited(input, fieldNumber, result);
                    break;
                default:
                    if (_options.ThrowOnError)
                        throw new NotSupportedException($"Wire type {wireType} is not supported.");
                    break;
            }
        }
        catch (Exception ex)
        {
            if (_options.ThrowOnError)
                throw new ProtobufDeserializationException($"Error deserializing field {fieldNumber} with wire type {wireType}.", ex);
        }
    }
    
    private void DeserializeVarint(ICodedInputStream input, int fieldNumber, DynamicProtobufResult result)
    {
        var position = input.Position;
        
        if (_options.IncludeAllPossibleTypes)
        {
            AddOrUpdateDynamicValue(result, $"Int32_{fieldNumber}", input.ReadInt32());
            
            input.SetPosition(position);
            AddOrUpdateDynamicValue(result, $"Int64_{fieldNumber}", input.ReadInt64());
            
            input.SetPosition(position);
            AddOrUpdateDynamicValue(result, $"UInt32_{fieldNumber}", input.ReadUInt32());
            
            input.SetPosition(position);
            AddOrUpdateDynamicValue(result, $"UInt64_{fieldNumber}", input.ReadUInt64());
            
            input.SetPosition(position);
            AddOrUpdateDynamicValue(result, $"SInt32_{fieldNumber}", input.ReadSInt32());
            
            input.SetPosition(position);
            AddOrUpdateDynamicValue(result, $"SInt64_{fieldNumber}", input.ReadSInt64());
            
            input.SetPosition(position);
            AddOrUpdateDynamicValue(result, $"Bool_{fieldNumber}", input.ReadBool());
            
            input.SetPosition(position);
            AddOrUpdateDynamicValue(result, $"Enum_{fieldNumber}", input.ReadInt32());
        }
        else
        {
            AddOrUpdateDynamicValue(result, $"Value_{fieldNumber}", input.ReadInt64());
        }
    }
    
    private void DeserializeFixed32(ICodedInputStream input, int fieldNumber, DynamicProtobufResult result)
    {
        var position = input.Position;
        
        if (_options.IncludeAllPossibleTypes)
        {
            AddOrUpdateDynamicValue(result, $"Fixed32_{fieldNumber}", input.ReadFixed32());
            
            input.SetPosition(position);
            AddOrUpdateDynamicValue(result, $"SFixed32_{fieldNumber}", input.ReadSFixed32());
            
            input.SetPosition(position);
            AddOrUpdateDynamicValue(result, $"Float_{fieldNumber}", input.ReadFloat());
        }
        else
        {
            AddOrUpdateDynamicValue(result, $"Fixed32_{fieldNumber}", input.ReadFixed32());
        }
    }
    
    private void DeserializeFixed64(ICodedInputStream input, int fieldNumber, DynamicProtobufResult result)
    {
        var position = input.Position;
        
        if (_options.IncludeAllPossibleTypes)
        {
            AddOrUpdateDynamicValue(result, $"Fixed64_{fieldNumber}", input.ReadFixed64());
            
            input.SetPosition(position);
            AddOrUpdateDynamicValue(result, $"SFixed64_{fieldNumber}", input.ReadSFixed64());
            
            input.SetPosition(position);
            AddOrUpdateDynamicValue(result, $"Double_{fieldNumber}", input.ReadDouble());
        }
        else
        {
            AddOrUpdateDynamicValue(result, $"Fixed64_{fieldNumber}", input.ReadFixed64());
        }
    }
    
    private void DeserializeLengthDelimited(ICodedInputStream input, int fieldNumber, DynamicProtobufResult result)
    {
        var bytes = input.ReadBytes();
        AddOrUpdateDynamicValue(result, $"Bytes_{fieldNumber}", bytes);
        AddOrUpdateDynamicValue(result, $"String_{fieldNumber}", _options.StringEncoding.GetString(bytes));
        TryParsePackedField(bytes, fieldNumber, result);
        if (!_options.AttemptNestedMessageDeserialization || _currentNestingDepth > _options.MaxNestingDepth) 
            return;
        
        try
        {
            var nestedInput = new CodedInputStream(bytes);
            AddOrUpdateDynamicValue(result, $"Message_{fieldNumber}", Deserialize(nestedInput));
        }
        catch (Exception ex)
        {
            if (_options.ThrowOnError)
                throw new ProtobufDeserializationException($"Error deserializing nested message in field {fieldNumber}.", ex);
        }
    }
    
    private bool TryParsePackedField(byte[] bytes, int fieldNumber, DynamicProtobufResult result)
    {
        if (bytes.Length == 0)
            return false;
            
        var packedInput = new CodedInputStream(bytes);
        var parsedAny = false;
        
        try
        {
            if (TryParsePackedVarint(packedInput, fieldNumber, result))
                parsedAny = true;
            
            packedInput.SetPosition(0);
            if (TryParsePackedFixed32(packedInput, fieldNumber, result))
                parsedAny = true;
            
            packedInput.SetPosition(0);
            if (TryParsePackedFixed64(packedInput, fieldNumber, result))
                parsedAny = true;
            
            return parsedAny;
        }
        catch
        {
            return parsedAny;
        }
    }
    
    private bool TryParsePackedVarint(ICodedInputStream input, int fieldNumber, DynamicProtobufResult result)
    {
        if (input.IsAtEnd())
            return false;
        
        var int32Values = new List<int>();
        var int64Values = new List<long>();
        var uint32Values = new List<uint>();
        var uint64Values = new List<ulong>();
        var sint32Values = new List<int>();
        var sint64Values = new List<long>();
        var boolValues = new List<bool>();
        var enumValues = new List<int>();
        
        try
        {
            while (!input.IsAtEnd())
            {
                var position = input.Position;
                
                var int32Value = input.ReadInt32();
                int32Values.Add(int32Value);
                
                input.SetPosition(position);
                var int64Value = input.ReadInt64();
                int64Values.Add(int64Value);
                
                input.SetPosition(position);
                var uint32Value = input.ReadUInt32();
                uint32Values.Add(uint32Value);
                
                input.SetPosition(position);
                var uint64Value = input.ReadUInt64();
                uint64Values.Add(uint64Value);
                
                input.SetPosition(position);
                var sint32Value = input.ReadSInt32();
                sint32Values.Add(sint32Value);
                
                input.SetPosition(position);
                var sint64Value = input.ReadSInt64();
                sint64Values.Add(sint64Value);
                
                input.SetPosition(position);
                var boolValue = input.ReadBool();
                boolValues.Add(boolValue);
                
                input.SetPosition(position);
                var enumValue = input.ReadInt32();
                enumValues.Add(enumValue);
            }
            
            if (int32Values.Count > 0)
            {
                if (_options.IncludeAllPossibleTypes)
                {
                    AddOrUpdateDynamicValue(result, $"Int32_{fieldNumber}", int32Values);
                    AddOrUpdateDynamicValue(result, $"Int64_{fieldNumber}", int64Values);
                    AddOrUpdateDynamicValue(result, $"UInt32_{fieldNumber}", uint32Values);
                    AddOrUpdateDynamicValue(result, $"UInt64_{fieldNumber}", uint64Values);
                    AddOrUpdateDynamicValue(result, $"SInt32_{fieldNumber}", sint32Values);
                    AddOrUpdateDynamicValue(result, $"SInt64_{fieldNumber}", sint64Values);
                    AddOrUpdateDynamicValue(result, $"Bool_{fieldNumber}", boolValues);
                    AddOrUpdateDynamicValue(result, $"Enum_{fieldNumber}", enumValues);
                }
                else
                {
                    AddOrUpdateDynamicValue(result, $"Int32_{fieldNumber}", int32Values);
                }
                
                return true;
            }
            
            return false;
        }
        catch
        {
            return false;
        }
    }
    
    private bool TryParsePackedFixed32(ICodedInputStream input, int fieldNumber, DynamicProtobufResult result)
    {
        if (input.IsAtEnd())
            return false;
            
        var fixed32Values = new List<uint>();
        var sfixed32Values = new List<int>();
        var floatValues = new List<float>();
        
        try
        {
            while (!input.IsAtEnd())
            {
                var position = input.Position;
                
                var fixed32Value = input.ReadFixed32();
                fixed32Values.Add(fixed32Value);
                
                input.SetPosition(position);
                var sfixed32Value = input.ReadSFixed32();
                sfixed32Values.Add(sfixed32Value);
                
                input.SetPosition(position);
                var floatValue = input.ReadFloat();
                floatValues.Add(floatValue);
            }
            
            if (fixed32Values.Count > 0)
            {
                if (_options.IncludeAllPossibleTypes)
                {
                    AddOrUpdateDynamicValue(result, $"Fixed32_{fieldNumber}", fixed32Values);
                    AddOrUpdateDynamicValue(result, $"SFixed32_{fieldNumber}", sfixed32Values);
                    AddOrUpdateDynamicValue(result, $"Float_{fieldNumber}", floatValues);
                }
                else
                {
                    AddOrUpdateDynamicValue(result, $"Fixed32_{fieldNumber}", fixed32Values);
                }
                
                return true;
            }
            
            return false;
        }
        catch
        {
            return false;
        }
    }
    
    private bool TryParsePackedFixed64(ICodedInputStream input, int fieldNumber, DynamicProtobufResult result)
    {
        if (input.IsAtEnd())
            return false;
            
        var fixed64Values = new List<ulong>();
        var sfixed64Values = new List<long>();
        var doubleValues = new List<double>();
        
        try
        {
            while (!input.IsAtEnd())
            {
                var position = input.Position;
                
                var fixed64Value = input.ReadFixed64();
                fixed64Values.Add(fixed64Value);
                
                input.SetPosition(position);
                var sfixed64Value = input.ReadSFixed64();
                sfixed64Values.Add(sfixed64Value);
                
                input.SetPosition(position);
                var doubleValue = input.ReadDouble();
                doubleValues.Add(doubleValue);
            }
            
            if (fixed64Values.Count > 0)
            {
                if (_options.IncludeAllPossibleTypes)
                {
                    AddOrUpdateDynamicValue(result, $"Fixed64_{fieldNumber}", fixed64Values);
                    AddOrUpdateDynamicValue(result, $"SFixed64_{fieldNumber}", sfixed64Values);
                    AddOrUpdateDynamicValue(result, $"Double_{fieldNumber}", doubleValues);
                }
                else
                {
                    AddOrUpdateDynamicValue(result, $"Fixed64_{fieldNumber}", fixed64Values);
                }
                
                return true;
            }
            
            return false;
        }
        catch
        {
            return false;
        }
    }
    
    /// <summary>
    /// Adds a value to the dynamic object, handling repeated fields if enabled.
    /// </summary>
    /// <param name="result">The dynamic result object.</param>
    /// <param name="propertyName">The property name.</param>
    /// <param name="value">The value to add.</param>
    private void AddOrUpdateDynamicValue(DynamicProtobufResult result, string propertyName, object? value)
    {
        if (!_options.HandleRepeatedFields)
        {
            result.Set(propertyName, value);
            return;
        }
        
        var existingValue = result.GetPropertyValue(propertyName);
        if (existingValue != null)
        {
            if (existingValue is List<object?> list)
            {
                list.Add(value);
            }
            else
            {
                var newList = new List<object?> { existingValue, value };
                result.Set(propertyName, newList);
                
                var repeatedPropertyName = $"{propertyName}_Array";
                result.Set(repeatedPropertyName, newList);
            }
        }
        else
        {
            result.Set(propertyName, value);
        }
    }
}