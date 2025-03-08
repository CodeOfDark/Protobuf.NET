using Protobuf.Parser.Exceptions;
using Protobuf.Parser.Models;
using Protobuf.Parser.Validation;

namespace Protobuf.Parser.Tests.Validation;

public class ProtoValidatorTests
{
    private readonly ProtoValidator _validator = new();

    [Fact]
    public void ValidProtoFile()
    {
        var protoFile = new ProtoFile
        {
            Syntax = "proto3",
            Package = "example.test",
            Messages =
            [
                new ProtoMessage()
                {
                    Name = "Person",
                    Fields =
                    [
                        new ProtoField() { Name = "name", Type = "string", Number = 1 },
                        new ProtoField() { Name = "id", Type = "int32", Number = 2 }
                    ]
                }
            ]
        };
            
        var exception = Record.Exception(() => _validator.Validate(protoFile));
        Assert.Null(exception);
    }
        
    [Theory]
    [InlineData("proto1")]
    [InlineData("proto4")]
    [InlineData("invalid")]
    public void InvalidSyntax(string syntax)
    {
        var protoFile = new ProtoFile
        { 
            Syntax = syntax
        };
            
        var exception = Assert.Throws<ProtoValidationException>(() => _validator.Validate(protoFile));
        Assert.Contains($"Invalid syntax '{syntax}'", exception.ValidationErrors[0]);
    }
        
    [Fact]
    public void InvalidPackageName()
    {
        var protoFile = new ProtoFile
        {
            Syntax = "proto3",
            Package = "invalid-package"
        };
            
        var exception = Assert.Throws<ProtoValidationException>(() => _validator.Validate(protoFile));
        Assert.Contains("Invalid package name", exception.ValidationErrors[0]);
    }
        
    [Fact]
    public void EmptyImportPath()
    {
        var protoFile = new ProtoFile
        {
            Syntax = "proto3",
            Imports = [
                new ProtoImport { Path = "" }
            ]
        };
            
        var exception = Assert.Throws<ProtoValidationException>(() => _validator.Validate(protoFile));
        Assert.Contains("Import path cannot be empty", exception.ValidationErrors[0]);
    }
        
    [Fact]
    public void InvalidMessageName()
    {
        var protoFile = new ProtoFile
        {
            Syntax = "proto3",
            Messages = [
                new ProtoMessage { Name = "1InvalidName" }
            ]
        };
            
        var exception = Assert.Throws<ProtoValidationException>(() => _validator.Validate(protoFile));
        Assert.Contains("Invalid message name", exception.ValidationErrors[0]);
    }
        
    [Fact]
    public void DuplicateFieldNumbers()
    {
        var protoFile = new ProtoFile
        {
            Syntax = "proto3",
            Messages =
            [
                new ProtoMessage
                {
                    Name = "Person",
                    Fields =
                    [
                        new ProtoField { Name = "name", Type = "string", Number = 1 },
                        new ProtoField { Name = "id", Type = "int32", Number = 1 }
                    ]
                }
            ]
        };
            
        var exception = Assert.Throws<ProtoValidationException>(() => _validator.Validate(protoFile));
        Assert.Contains("Duplicate field number", exception.ValidationErrors[0]);
    }
        
    [Fact]
    public void RequiredFieldInProto3()
    {
        var protoFile = new ProtoFile
        {
            Syntax = "proto3",
            Messages =
            [
                new ProtoMessage
                {
                    Name = "Person",
                    Fields = [
                        new ProtoField { Name = "name", Type = "string", Number = 1, Rule = "required" }
                    ]
                }
            ]
        };
            
        var exception = Assert.Throws<ProtoValidationException>(() => _validator.Validate(protoFile));
        Assert.Contains("cannot be 'required' in proto3", exception.ValidationErrors[0]);
    }
        
    [Fact]
    public void EmptyEnum()
    {
        var protoFile = new ProtoFile
        {
            Syntax = "proto3",
            Enums = [
                new ProtoEnum { Name = "Status" }
            ]
        };
            
        var exception = Assert.Throws<ProtoValidationException>(() => _validator.Validate(protoFile));
        Assert.Contains("Enum 'Status' must contain at least one value", exception.ValidationErrors[0]);
    }
        
    [Fact]
    public void DuplicateEnumValueNames()
    {
        var protoFile = new ProtoFile
        {
            Syntax = "proto3",
            Enums =
            [
                new ProtoEnum
                {
                    Name = "Status",
                    Values =
                    [
                        new ProtoEnumValue { Name = "ACTIVE", Number = 1 },
                        new ProtoEnumValue { Name = "ACTIVE", Number = 2 }
                    ]
                }
            ]
        };
            
        var exception = Assert.Throws<ProtoValidationException>(() => _validator.Validate(protoFile));
        Assert.Contains("Duplicate enum value name", exception.ValidationErrors[0]);
    }
        
    [Fact]
    public void EmptyOneofField()
    {
        var protoFile = new ProtoFile
        {
            Syntax = "proto3",
            Messages =
            [
                new ProtoMessage
                {
                    Name = "Person",
                    Oneofs = [
                        new ProtoOneof { Name = "contact" }
                    ]
                }
            ]
        };
            
        var exception = Assert.Throws<ProtoValidationException>(() => _validator.Validate(protoFile));
        Assert.Contains("Oneof 'contact' must contain at least one field", exception.ValidationErrors[0]);
    }
        
    [Fact]
    public void MultipleFiles()
    {
        var protoFiles = new List<ProtoFile>
        {
            new()
            {
                FileName = "file1.proto",
                Syntax = "proto3",
                Messages = [
                    new ProtoMessage { Name = "1InvalidName" }
                ]
            },
            new()
            {
                FileName = "file2.proto",
                Syntax = "invalid",
                Package = "example"
            }
        };
            
        var exception = Assert.Throws<ProtoValidationException>(() => _validator.ValidateAll(protoFiles));
        Assert.Contains("Validation errors in file 'file1.proto'", exception.ValidationErrors[0]);
        Assert.Contains("Validation errors in file 'file2.proto'", exception.ValidationErrors[2]);
    }
}