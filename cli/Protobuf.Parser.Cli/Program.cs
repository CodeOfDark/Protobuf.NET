using Protobuf.Parser.Exceptions;
using Protobuf.Parser.Models;
using Protobuf.Parser.Validation;

namespace Protobuf.Parser.Cli;

internal static class Program
{
    private static void Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("Please provide a directory to the *.proto files.");
            Console.WriteLine("Usage: Protobuf.Parser.Cli <path-to-proto-directory>");
            return;
        }

        var protoPath = args[0];
            
        try
        {
            var parser = new ProtoParser();
            var protoValidator = new ProtoValidator();
            var protoFiles = parser.ParseDirectory(protoPath);
            protoValidator.ValidateAll(protoFiles);
            
            DisplayProtoFiles(protoFiles);
            
            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }
        catch (ProtoValidationException ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            foreach (var validationError in ex.ValidationErrors)
                Console.WriteLine($"Validation Error: {validationError}");
            
            Console.ResetColor();
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Error parsing proto file: {ex.Message}");
            Console.ResetColor();
        }
    }

    private static void DisplayProtoFiles(IEnumerable<ProtoFile> protoFiles)
    {
        Console.Clear();
        
        foreach (var protoFile in protoFiles)
            DisplayProtoFile(protoFile);
    }

    private static void DisplayProtoFile(ProtoFile protoFile)
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("=".PadRight(80, '='));
        Console.WriteLine($"Protocol Buffer File: {protoFile.FileName}");
        Console.WriteLine("=".PadRight(80, '='));
        Console.ResetColor();
        
        Console.WriteLine($"Syntax: {protoFile.Syntax}");
        Console.WriteLine($"Package: {protoFile.Package}");
        
        if (protoFile.Imports.Count > 0)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\nImports:");
            Console.ResetColor();
                
            foreach (var import in protoFile.Imports)
            {
                var importType = import.IsPublic ? "public" : (import.IsWeak ? "weak" : "");
                Console.WriteLine($"  {importType} \"{import.Path}\"");
            }
        }
        
        if (protoFile.Options.Count > 0)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\nOptions:");
            Console.ResetColor();
                
            foreach (var option in protoFile.Options)
                Console.WriteLine($"  {option.Name} = {option.Value}");
        }
        
        if (protoFile.Enums.Count > 0)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\nEnums:");
            Console.ResetColor();
                
            foreach (var protoEnum in protoFile.Enums)
                DisplayEnum(protoEnum, 1);
        }
        
        if (protoFile.Messages.Count > 0)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\nMessages:");
            Console.ResetColor();
                
            foreach (var message in protoFile.Messages)
                DisplayMessage(message, 1);
        }
        
        if (protoFile.Services.Count > 0)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\nServices:");
            Console.ResetColor();
                
            foreach (var service in protoFile.Services)
                DisplayService(service, 1);
        }
        
        if (protoFile.Extensions.Count > 0)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\nExtensions:");
            Console.ResetColor();
                
            foreach (var extension in protoFile.Extensions)
                DisplayExtension(extension, 1);
        }
    }

    private static void DisplayEnum(ProtoEnum protoEnum, int indentLevel)
    {
        var indent = new string(' ', indentLevel * 2);
        var parentInfo = string.IsNullOrEmpty(protoEnum.ParentMessage) ? "" : $" (in {protoEnum.ParentMessage})";
            
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"{indent}enum {protoEnum.Name}{parentInfo} {{");
        Console.ResetColor();
        
        foreach (var value in protoEnum.Values)
        {
            var optionsText = "";
            if (value.Options.Count > 0)
                optionsText = " [" + string.Join(", ", value.Options.Select(o => $"{o.Name} = {o.Value}")) + "]";
                
            Console.WriteLine($"{indent}  {value.Name} = {value.Number}{optionsText};");
        }
        
        if (protoEnum.Options.Count > 0)
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine($"{indent}  // Options:");
            Console.ResetColor();
                
            foreach (var option in protoEnum.Options)
                Console.WriteLine($"{indent}  option {option.Name} = {option.Value};");
        }
        
        Console.WriteLine($"{indent}}}");
    }

    private static void DisplayMessage(ProtoMessage message, int indentLevel)
    {
        var indent = new string(' ', indentLevel * 2);
        var parentInfo = string.IsNullOrEmpty(message.ParentMessage) ? "" : $" (in {message.ParentMessage})";
            
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"{indent}message {message.Name}{parentInfo} {{");
        Console.ResetColor();
        
        if (message.Options.Count > 0)
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine($"{indent}  // Options:");
            Console.ResetColor();
                
            foreach (var option in message.Options)
                Console.WriteLine($"{indent}  option {option.Name} = {option.Value};");
        }
        
        if (message.Fields.Count > 0)
        {
            if (message.Options.Count > 0)
                Console.WriteLine();
                
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine($"{indent}  // Fields:");
            Console.ResetColor();
                
            foreach (var field in message.Fields)
                DisplayField(field, indentLevel + 1);
        }
        
        if (message.Oneofs.Count > 0)
        {
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine($"{indent}  // Oneofs:");
            Console.ResetColor();
                
            foreach (var oneof in message.Oneofs)
            {
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine($"{indent}  oneof {oneof.Name} {{");
                Console.ResetColor();
                    
                foreach (var field in oneof.Fields)
                    DisplayField(field, indentLevel + 2);
                    
                Console.WriteLine($"{indent}  }}");
            }
        }
        
        if (message.NestedEnums.Count > 0)
        {
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine($"{indent}  // Nested Enums:");
            Console.ResetColor();
                
            foreach (var nestedEnum in message.NestedEnums)
                DisplayEnum(nestedEnum, indentLevel + 1);
        }
        
        if (message.NestedMessages.Count > 0)
        {
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine($"{indent}  // Nested Messages:");
            Console.ResetColor();
                
            foreach (var nestedMessage in message.NestedMessages)
                DisplayMessage(nestedMessage, indentLevel + 1);
        }
        
        if (message.Reserved.Count > 0)
        {
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine($"{indent}  // Reserved:");
            Console.ResetColor();
                
            foreach (var reserved in message.Reserved)
            {
                if (reserved.IsName)
                {
                    var names = string.Join(", ", reserved.FieldNames.Select(n => $"\"{n}\""));
                    Console.WriteLine($"{indent}  reserved {names};");
                }
                else
                {
                    var ranges = reserved.Ranges.Select(r => r.IsSingleNumber ? r.From.ToString() : $"{r.From} to {r.To}");
                    Console.WriteLine($"{indent}  reserved {string.Join(", ", ranges)};");
                }
            }
        }
            
        Console.WriteLine($"{indent}}}");
    }

    private static void DisplayField(ProtoField field, int indentLevel)
    {
        var indent = new string(' ', indentLevel * 2);
            
        string fieldType;
        if (field.IsMap)
            fieldType = $"map<{field.KeyType}, {field.Type}>";
        else if (!string.IsNullOrEmpty(field.GenericType))
            fieldType = $"{field.Type}<{field.GenericType}>";
        else
            fieldType = field.Type;
            
        var rule = string.IsNullOrEmpty(field.Rule) ? "" : field.Rule + " ";
        var optionsText = "";
            
        if (field.Options.Count > 0)
            optionsText = " [" + string.Join(", ", field.Options.Select(o => $"{o.Name} = {o.Value}")) + "]";
            
        Console.WriteLine($"{indent}{rule}{fieldType} {field.Name} = {field.Number}{optionsText};");
    }

    private static void DisplayService(ProtoService service, int indentLevel)
    {
        var indent = new string(' ', indentLevel * 2);
            
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"{indent}service {service.Name} {{");
        Console.ResetColor();
        
        if (service.Options.Count > 0)
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine($"{indent}  // Options:");
            Console.ResetColor();
                
            foreach (var option in service.Options)
                Console.WriteLine($"{indent}  option {option.Name} = {option.Value};");
                
            if (service.Methods.Count > 0)
                Console.WriteLine();
        }
        
        foreach (var method in service.Methods)
        {
            var clientStream = method.ClientStreaming ? "stream " : "";
            var serverStream = method.ServerStreaming ? "stream " : "";
            
            Console.WriteLine($"{indent}  rpc {method.Name}({clientStream}{method.InputType}) returns ({serverStream}{method.OutputType})");
            
            if (method.Options.Count > 0)
            {
                Console.WriteLine($"{indent}  {{");
                    
                foreach (var option in method.Options)
                    Console.WriteLine($"{indent}    option {option.Name} = {option.Value};");
                    
                Console.WriteLine($"{indent}  }};");
            }
            else
            {
                Console.WriteLine($"{indent}  ;");
            }
        }
            
        Console.WriteLine($"{indent}}}");
    }

    private static void DisplayExtension(ProtoExtension extension, int indentLevel)
    {
        var indent = new string(' ', indentLevel * 2);
            
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"{indent}extend {extension.ExtendedType} {{");
        Console.ResetColor();
            
        foreach (var field in extension.Fields)
            DisplayField(field, indentLevel + 1);
            
        Console.WriteLine($"{indent}}}");
    }
}