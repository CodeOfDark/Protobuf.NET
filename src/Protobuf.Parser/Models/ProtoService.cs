namespace Protobuf.Parser.Models;

/// <summary>
/// Represents a service in a Protocol Buffer file.
/// </summary>
public class ProtoService
{
    /// <summary>
    /// Gets or sets the service name.
    /// </summary>
    public string Name { get; set; } = "";

    /// <summary>
    /// Gets or sets the list of methods in the service.
    /// </summary>
    public List<ProtoMethod> Methods { get; set; } = [];

    /// <summary>
    /// Gets or sets the list of service options.
    /// </summary>
    public List<ProtoOption> Options { get; set; } = [];
        
    /// <summary>
    /// Gets the method with the specified name.
    /// </summary>
    /// <param name="name">The name of the method to get.</param>
    /// <returns>The method with the specified name, or <c>null</c> if no such method exists.</returns>
    public ProtoMethod? GetMethod(string name) => Methods.FirstOrDefault(method => method.Name == name);
}