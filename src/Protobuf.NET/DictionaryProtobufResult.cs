using System.Text;
using Protobuf.NET.Interfaces;

namespace Protobuf.NET;

/// <summary>
/// Represents a dictionary-based Protocol Buffers deserialized result.
/// </summary>
/// <remarks>
/// This class extends <see cref="Dictionary{TKey, TValue}"/> with additional methods
/// for retrieving typed values from the deserialized data.
/// </remarks>
public class DictionaryProtobufResult : Dictionary<string, object>, IProtobufResult
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DictionaryProtobufResult"/> class.
    /// </summary>
    public DictionaryProtobufResult() : base()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DictionaryProtobufResult"/> class
    /// with the specified initial capacity.
    /// </summary>
    /// <param name="capacity">The initial capacity of the dictionary.</param>
    public DictionaryProtobufResult(int capacity) : base(capacity)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DictionaryProtobufResult"/> class
    /// with the elements copied from the specified dictionary.
    /// </summary>
    /// <param name="dictionary">The dictionary whose elements are copied to the new dictionary.</param>
    public DictionaryProtobufResult(IDictionary<string, object> dictionary) : base(dictionary)
    {
    }

    /// <summary>
    /// Gets the value associated with the specified field number and type.
    /// </summary>
    /// <param name="fieldNumber">The field number.</param>
    /// <param name="typeName">The type name (e.g., "int32", "string").</param>
    /// <returns>The value if found; otherwise, null.</returns>
    public object? GetValue(int fieldNumber, string typeName)
    {
        return TryGetValue($"{fieldNumber}({typeName})", out var value) ? value : null;
    }

    /// <summary>
    /// Gets the value associated with the specified field number and type, converted to the specified type.
    /// </summary>
    /// <typeparam name="T">The type to convert the value to.</typeparam>
    /// <param name="fieldNumber">The field number.</param>
    /// <param name="typeName">The type name (e.g., "int32", "string").</param>
    /// <returns>The converted value if found and conversion is successful; otherwise, the default value for <typeparamref name="T"/>.</returns>
    public T? GetValue<T>(int fieldNumber, string typeName)
    {
        var value = GetValue(fieldNumber, typeName);
        switch (value)
        {
            case null:
                return default;
            case T typedValue:
                return typedValue;
            default:
                try
                {
                    return (T)Convert.ChangeType(value, typeof(T));
                }
                catch
                {
                    return default;
                }
        }
    }
    
    /// <summary>
    /// Gets a repeated field value as an array.
    /// </summary>
    /// <typeparam name="T">The type of elements in the array.</typeparam>
    /// <param name="fieldNumber">The field number.</param>
    /// <param name="typeName">The type name (e.g., "int32", "string").</param>
    /// <returns>An array of values if found; otherwise, an empty array.</returns>
    public T[] GetRepeatedValues<T>(int fieldNumber, string typeName)
    {
        var repeatedKey = $"{fieldNumber}({typeName})[]";
        if (TryGetValue(repeatedKey, out var repeatedValue) && repeatedValue is List<object> repeatedList)
            return repeatedList.Select(ConvertValue<T>).Where(item => item != null).Select(item => item!).ToArray();
        
        var value = GetValue(fieldNumber, typeName);
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
    /// Gets all field numbers present in the result.
    /// </summary>
    /// <returns>A collection of field numbers.</returns>
    public IEnumerable<int> GetFieldNumbers()
    {
        var fieldNumbers = new HashSet<int>();
        foreach (var match in Keys.Select(key => System.Text.RegularExpressions.Regex.Match(key, @"^(\d+)\(")))
        {
            if (match.Success && int.TryParse(match.Groups[1].Value, out var fieldNumber))
                fieldNumbers.Add(fieldNumber);
        }
        return fieldNumbers;
    }

    /// <summary>
    /// Gets all type names available for a specific field number.
    /// </summary>
    /// <param name="fieldNumber">The field number to get type names for.</param>
    /// <returns>A collection of type names.</returns>
    public IEnumerable<string> GetTypeNames(int fieldNumber)
    {
        var typeNames = new List<string>();
        foreach (var key in Keys)
        {
            var match = System.Text.RegularExpressions.Regex.Match(key, $@"^{fieldNumber}\((.*?)\)(?:\[\])?$");
            if (match.Success)
                typeNames.Add(match.Groups[1].Value);
        }
        return typeNames.Distinct();
    }

    /// <summary>
    /// Gets a nested message from the result.
    /// </summary>
    /// <param name="fieldNumber">The field number of the nested message.</param>
    /// <returns>The nested message if found; otherwise, null.</returns>
    public DictionaryProtobufResult? GetNestedMessage(int fieldNumber)
    {
        var value = GetValue(fieldNumber, "message");
        return value as DictionaryProtobufResult;
    }
    
    /// <summary>
    /// Gets repeated nested messages from the result.
    /// </summary>
    /// <param name="fieldNumber">The field number of the nested messages.</param>
    /// <returns>An array of nested messages if found; otherwise, an empty array.</returns>
    public DictionaryProtobufResult[] GetRepeatedNestedMessages(int fieldNumber)
    {
        var repeatedKey = $"{fieldNumber}(message)[]";
        if (TryGetValue(repeatedKey, out var repeatedValue) && repeatedValue is List<object> repeatedList)
        {
            return repeatedList
                .OfType<DictionaryProtobufResult>()
                .ToArray();
        }
        
        var value = GetValue(fieldNumber, "message");
        if (value == null)
            return [];
            
        if (value is List<object> valueList)
            return valueList.OfType<DictionaryProtobufResult>().ToArray();
        
        return value is DictionaryProtobufResult message ? [message] : [];
    }

    /// <summary>
    /// Returns a formatted string representation of this result.
    /// </summary>
    /// <returns>A string representation of this result.</returns>
    public override string ToString()
    {
        var builder = new StringBuilder();
        builder.AppendLine("{");
        
        foreach (var fieldNumber in GetFieldNumbers())
        {
            builder.AppendLine($"  Field {fieldNumber}:");
            foreach (var typeName in GetTypeNames(fieldNumber))
            {
                var key = $"{fieldNumber}({typeName})";
                var repeatedKey = $"{key}[]";
                
                if (TryGetValue(repeatedKey, out var repeatedValue) && repeatedValue is List<object>)
                {
                    builder.AppendLine($"    {typeName}[]: {FormatValue(repeatedValue)}");
                }
                else
                {
                    var value = GetValue(fieldNumber, typeName);
                    if (value is List<object>)
                        builder.AppendLine($"    {typeName}[]: {FormatValue(value)}");
                    else
                        builder.AppendLine($"    {typeName}: {FormatValue(value)}");
                }
            }
        }
        
        builder.AppendLine("}");
        return builder.ToString();
    }

    private static string FormatValue(object? value)
    {
        return value switch
        {
            null => "null",
            string str => $"\"{str}\"",
            byte[] bytes => $"[{bytes.Length} bytes]",
            DictionaryProtobufResult nested => $"{{{nested.GetFieldNumbers().Count()} fields}}",
            List<object> list => $"[{list.Count} items: {string.Join(", ", list.Select(FormatValue))}]",
            _ => value.ToString() ?? "null"
        };
    }
}