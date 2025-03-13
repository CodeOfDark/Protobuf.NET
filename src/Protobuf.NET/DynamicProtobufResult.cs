using System.Dynamic;
using System.Text;
using Protobuf.NET.Interfaces;

namespace Protobuf.NET;

/// <summary>
/// Represents a dynamic object-based Protocol Buffers deserialized result.
/// </summary>
/// <remarks>
/// This class provides a dynamic interface to access Protocol Buffers data
/// using property-like syntax.
/// </remarks>
public class DynamicProtobufResult : DynamicObject, IProtobufResult
{
    private readonly Dictionary<string, object?> _properties = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="DynamicProtobufResult"/> class.
    /// </summary>
    public DynamicProtobufResult()
    {
    }

    /// <summary>
    /// Tries to get a member from the dynamic object.
    /// </summary>
    /// <param name="binder">The binder provided by the call site.</param>
    /// <param name="result">The result of the get operation.</param>
    /// <returns>True if the operation is successful; otherwise, false.</returns>
    public override bool TryGetMember(GetMemberBinder binder, out object? result)
    {
        if (_properties.TryGetValue(binder.Name, out result)) 
            return true;
        
        result = new DynamicProtobufResult();
        _properties[binder.Name] = result;
        return true;
    }

    /// <summary>
    /// Tries to set a member on the dynamic object.
    /// </summary>
    /// <param name="binder">The binder provided by the call site.</param>
    /// <param name="value">The value to set.</param>
    /// <returns>True if the operation is successful; otherwise, false.</returns>
    public override bool TrySetMember(SetMemberBinder binder, object? value)
    {
        _properties[binder.Name] = value;
        return true;
    }

    /// <summary>
    /// Fluent API to set a property value.
    /// </summary>
    /// <param name="propertyName">The name of the property to set.</param>
    /// <param name="value">The value to set.</param>
    /// <returns>This instance for method chaining.</returns>
    public DynamicProtobufResult Set(string propertyName, object? value)
    {
        _properties[propertyName] = value;
        return this;
    }
        
    /// <summary>
    /// Gets all property names in this object.
    /// </summary>
    /// <returns>A collection of property names.</returns>
    public IEnumerable<string> GetPropertyNames() => _properties.Keys;
        
    /// <summary>
    /// Gets a property value by name.
    /// </summary>
    /// <param name="propertyName">The name of the property to get.</param>
    /// <returns>The property value, or null if not found.</returns>
    public object? GetPropertyValue(string propertyName)
    {
        return _properties.GetValueOrDefault(propertyName);
    }
    
    /// <summary>
    /// Gets a property value by name, converted to the specified type.
    /// </summary>
    /// <typeparam name="T">The type to convert the value to.</typeparam>
    /// <param name="propertyName">The name of the property to get.</param>
    /// <returns>The converted value if found and conversion is successful; otherwise, the default value for <typeparamref name="T"/>.</returns>
    public T? GetPropertyValue<T>(string propertyName)
    {
        var value = GetPropertyValue(propertyName);
        if (value == null)
            return default;
        
        if (value is T typedValue)
            return typedValue;
        
        try
        {
            return (T)Convert.ChangeType(value, typeof(T));
        }
        catch
        {
            return default;
        }
    }
    
    /// <summary>
    /// Gets a repeated property value as an array.
    /// </summary>
    /// <typeparam name="T">The type of elements in the array.</typeparam>
    /// <param name="propertyName">The name of the property.</param>
    /// <returns>An array of values if found; otherwise, an empty array.</returns>
    public T[] GetRepeatedPropertyValues<T>(string propertyName)
    {
        var arrayPropertyName = $"{propertyName}_Array";
        if (_properties.TryGetValue(arrayPropertyName, out var arrayValue) && arrayValue is List<object> arrayList)
            return arrayList.Select(ConvertValue<T>).Where(item => item != null).Select(item => item!).ToArray();
        
        var value = GetPropertyValue(propertyName);
        if (value == null)
            return [];
            
        if (value is List<object> valueList)
            return valueList.Select(ConvertValue<T>).Where(item => item != null).Select(item => item!).ToArray();
        
        var singleValue = ConvertValue<T>(value);
        return singleValue != null ? [singleValue] : [];
    }
    
    private T? ConvertValue<T>(object? value)
    {
        if (value == null)
            return default;
            
        if (value is T typedValue)
            return typedValue;
            
        try
        {
            return (T)Convert.ChangeType(value, typeof(T));
        }
        catch
        {
            return default;
        }
    }
        
    /// <summary>
    /// Gets all properties as a dictionary.
    /// </summary>
    /// <returns>A dictionary containing all properties.</returns>
    public IDictionary<string, object?> GetAllProperties() => new Dictionary<string, object?>(_properties);

    /// <summary>
    /// Returns a string representation of this object.
    /// </summary>
    /// <returns>A string representation of this object.</returns>
    public override string ToString()
    {
        var builder = new StringBuilder();
        builder.AppendLine("{");
        
        foreach (var property in _properties)
        {
            builder.Append($"  {property.Key}: ");

            switch (property.Value)
            {
                case null:
                    builder.AppendLine("null");
                    break;
                case DynamicProtobufResult nested:
                    builder.AppendLine(nested.ToString().Replace(Environment.NewLine, Environment.NewLine + "  "));
                    break;
                case byte[] bytes:
                    builder.AppendLine($"[{bytes.Length} bytes]");
                    break;
                case List<object> list:
                    builder.AppendLine($"[{list.Count} items]");
                    foreach (var obj in list)
                    {
                        if (obj is DynamicProtobufResult nested)
                            builder.AppendLine(nested.ToString());
                        else
                            builder.AppendLine(obj.ToString());
                    }
                    break;
                default:
                    builder.AppendLine(property.Value.ToString());
                    break;
            }
        }
        
        builder.AppendLine("}");
        return builder.ToString();
    }
}