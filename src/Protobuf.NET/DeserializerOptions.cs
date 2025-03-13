using System.Text;

namespace Protobuf.NET;

/// <summary>
/// Provides options for configuring the Protocol Buffers deserialization process.
/// </summary>
public class DeserializerOptions
{
    /// <summary>
    /// Gets or sets whether to attempt nested message deserialization.
    /// When true, any length-delimited field will be attempted to be parsed as a nested message.
    /// </summary>
    /// <value>
    /// The default value is <c>true</c>.
    /// </value>
    public bool AttemptNestedMessageDeserialization { get; set; } = true;
    
    /// <summary>
    /// Gets or sets whether to include multiple interpretations of the same field.
    /// When true, each field will be deserialized as all compatible types.
    /// When false, only a single type interpretation will be included.
    /// </summary>
    /// <value>
    /// The default value is <c>true</c>.
    /// </value>
    public bool IncludeAllPossibleTypes { get; set; } = true;
    
    /// <summary>
    /// Gets or sets the encoding to use for string deserialization.
    /// </summary>
    /// <value>
    /// The default value is <see cref="Encoding.UTF8"/>.
    /// </value>
    public Encoding StringEncoding { get; set; } = Encoding.UTF8;
    
    /// <summary>
    /// Gets or sets whether to throw exceptions when deserialization errors occur.
    /// When false, errors will be silently ignored.
    /// </summary>
    /// <value>
    /// The default value is <c>false</c>.
    /// </value>
    public bool ThrowOnError { get; set; } = false;
    
    /// <summary>
    /// Gets or sets the maximum nesting depth for nested message deserialization.
    /// A value of 0 means no nesting, 1 means one level of nesting, etc.
    /// </summary>
    /// <value>
    /// The default value is <c>64</c>.
    /// </value>
    public int MaxNestingDepth { get; set; } = 64;
    
    /// <summary>
    /// Gets or sets whether to detect and handle repeated fields.
    /// When true, fields that appear multiple times will be collected into arrays/lists.
    /// When false, repeated fields will overwrite previous values.
    /// </summary>
    /// <value>
    /// The default value is <c>true</c>.
    /// </value>
    public bool HandleRepeatedFields { get; set; } = true;
    
    /// <summary>
    /// Gets or sets the suffix to use for repeated field keys in the dictionary result.
    /// Only relevant when <see cref="HandleRepeatedFields"/> is true.
    /// </summary>
    /// <value>
    /// The default value is <c>"[]"</c>, which results in keys like "1(int32)[]".
    /// </value>
    public string RepeatedFieldSuffix { get; set; } = "[]";

    /// <summary>
    /// Creates a copy of the current options.
    /// </summary>
    /// <returns>A new instance of <see cref="DeserializerOptions"/> with the same settings.</returns>
    public DeserializerOptions Clone()
    {
        return new DeserializerOptions
        {
            AttemptNestedMessageDeserialization = AttemptNestedMessageDeserialization,
            IncludeAllPossibleTypes = IncludeAllPossibleTypes,
            StringEncoding = StringEncoding,
            ThrowOnError = ThrowOnError,
            MaxNestingDepth = MaxNestingDepth,
            HandleRepeatedFields = HandleRepeatedFields,
            RepeatedFieldSuffix = RepeatedFieldSuffix
        };
    }
}