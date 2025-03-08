namespace Protobuf.Parser.Utilities;

/// <summary>
/// Utility class for working with file paths.
/// </summary>
public static class PathUtils
{
    /// <summary>
    /// Resolves an import path relative to a base file.
    /// </summary>
    /// <param name="basePath">The path of the file containing the import.</param>
    /// <param name="importPath">The import path to resolve.</param>
    /// <returns>The resolved import path.</returns>
    public static string ResolveImportPath(string basePath, string importPath)
    {
        if (Path.IsPathRooted(importPath))
            return importPath;
            
        var baseDirectory = Path.GetDirectoryName(basePath) ?? "";
        return Path.GetFullPath(Path.Combine(baseDirectory, importPath));
    }
    
    /// <summary>
    /// Gets the full path.
    /// </summary>
    /// <param name="path">The path of the proto file.</param>
    /// <returns>The resolved path.</returns>
    public static string ResolvePath(string path)
    {
        return Path.GetFullPath(path);
    }
}