using Protobuf.Parser.Exceptions;
using Protobuf.Parser.Models;
using Protobuf.Parser.Validation;

namespace Protobuf.Parser.Tests.Validation;

public class ReferenceValidatorTests
{
    private readonly ReferenceValidator _validator = new();

    [Fact]
    public void ValidReferences()
    {
        var protoFile = new ProtoFile
        {
            Package = "example",
            Messages =
            [
                new ProtoMessage
                {
                    Name = "Person",
                    Fields =
                    [
                        new ProtoField { Name = "name", Type = "string", Number = 1 },
                        new ProtoField { Name = "address", Type = "Address", Number = 2 }
                    ]
                },

                new ProtoMessage
                {
                    Name = "Address",
                    Fields = [
                        new ProtoField { Name = "street", Type = "string", Number = 1 }
                    ]
                }
            ]
        };
        
        var exception = Record.Exception(() => _validator.ValidateReferences(protoFile));
        Assert.Null(exception);
    }
        
    [Fact]
    public void UnknownType()
    {
        var protoFile = new ProtoFile
        {
            Package = "example",
            Messages =
            [
                new ProtoMessage
                {
                    Name = "Person",
                    Fields =
                    [
                        new ProtoField { Name = "name", Type = "string", Number = 1 },
                        new ProtoField { Name = "address", Type = "UnknownType", Number = 2 }
                    ]
                }
            ]
        };
        
        var exception = Assert.Throws<ProtoValidationException>(() => _validator.ValidateReferences(protoFile));
        Assert.Contains("Unknown type 'UnknownType'", exception.ValidationErrors[0]);
    }
        
    [Fact]
    public void UnknownServiceMethodType()
    {
        var protoFile = new ProtoFile
        {
            Package = "example",
            Messages =
            [
                new ProtoMessage
                {
                    Name = "Person",
                    Fields = [
                        new ProtoField { Name = "name", Type = "string", Number = 1 }
                    ]
                }
            ],
            Services =
            [
                new ProtoService
                {
                    Name = "PersonService",
                    Methods =
                    [
                        new ProtoMethod
                        {
                            Name = "GetPerson",
                            InputType = "UnknownRequest",
                            OutputType = "Person"
                        }
                    ]
                }
            ]
        };
        
        var exception = Assert.Throws<ProtoValidationException>(() => _validator.ValidateReferences(protoFile));
        Assert.Contains("Unknown input type 'UnknownRequest'", exception.ValidationErrors[0]);
    }
        
    [Fact]
    public void ExtendNonMessage()
    {
        var protoFile = new ProtoFile
        {
            Package = "example",
            Enums =
            [
                new ProtoEnum
                {
                    Name = "Status",
                    Values = [
                        new ProtoEnumValue { Name = "ACTIVE", Number = 1 }
                    ]
                }
            ],
            Extensions =
            [
                new ProtoExtension
                {
                    ExtendedType = "Status",
                    Fields = [
                        new ProtoField { Name = "extra", Type = "string", Number = 100 }
                    ]
                }
            ]
        };
        
        var exception = Assert.Throws<ProtoValidationException>(() => _validator.ValidateReferences(protoFile));
        Assert.Contains("Extended type 'Status' must be a message", exception.ValidationErrors[0]);
    }
        
    [Fact]
    public void MultipleFiles()
    {
        var protoFiles = new List<ProtoFile>
        {
            new()
            {
                FileName = "common.proto",
                Package = "common",
                Messages = [
                    new ProtoMessage { Name = "Address" }
                ]
            },
            new()
            {
                FileName = "user.proto",
                Package = "user",
                Messages =
                [
                    new ProtoMessage
                    {
                        Name = "User",
                        Fields = [
                            new ProtoField { Name = "address", Type = "common.Address", Number = 1 }
                        ]
                    }
                ]
            }
        };
        
        var exception = Record.Exception(() => _validator.ValidateReferences(protoFiles));
        Assert.Null(exception);
    }
}