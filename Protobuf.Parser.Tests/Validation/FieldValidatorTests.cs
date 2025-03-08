using Protobuf.Parser.Models;
using Protobuf.Parser.Validation;

namespace Protobuf.Parser.Tests.Validation;

public class FieldValidatorTests
{
    [Fact]
    public void ValidField()
    {
        var field = new ProtoField
        {
            Name = "name",
            Type = "string",
            Number = 1
        };
            
        var errors = FieldValidator.ValidateField(field, "proto3").ToList();
        Assert.Empty(errors);
    }
        
    [Fact]
    public void EmptyName()
    {
        var field = new ProtoField
        {
            Name = "",
            Type = "string",
            Number = 1
        };
            
        var errors = FieldValidator.ValidateField(field, "proto3").ToList();
        Assert.Single(errors);
        Assert.Contains("Field name cannot be empty", errors[0]);
    }
        
    [Fact]
    public void InvalidName()
    {
        var field = new ProtoField
        {
            Name = "1invalid",
            Type = "string",
            Number = 1
        };
        
        var errors = FieldValidator.ValidateField(field, "proto3").ToList();
        Assert.Single(errors);
        Assert.Contains("Invalid field name", errors[0]);
    }
        
    [Fact]
    public void EmptyType()
    {
        var field = new ProtoField
        {
            Name = "name",
            Type = "",
            Number = 1
        };
            
        var errors = FieldValidator.ValidateField(field, "proto3").ToList();
        Assert.Single(errors);
        Assert.Contains("Type for field 'name' cannot be empty", errors[0]);
    }
        
    [Fact]
    public void InvalidNumber()
    {
        var field = new ProtoField
        {
            Name = "name",
            Type = "string",
            Number = 0
        };
            
        var errors = FieldValidator.ValidateField(field, "proto3").ToList();
        Assert.Single(errors);
        Assert.Contains("Field number for field 'name' must be greater than 0", errors[0]);
    }
        
    [Fact]
    public void ReservedNumber()
    {
        var field = new ProtoField
        {
            Name = "name",
            Type = "string",
            Number = 19000
        };
            
        var errors = FieldValidator.ValidateField(field, "proto3").ToList();
        Assert.Single(errors);
        Assert.Contains("Field number 19000 for field 'name' is in the reserved range", errors[0]);
    }
        
    [Fact]
    public void RequiredInProto3()
    {
        var field = new ProtoField
        {
            Name = "name",
            Type = "string",
            Number = 1,
            Rule = "required"
        };
            
        var errors = FieldValidator.ValidateField(field, "proto3").ToList();
        Assert.Single(errors);
        Assert.Contains("Field 'name' cannot be 'required' in proto3", errors[0]);
    }
        
    [Fact]
    public void OptionalInProto3()
    {
        var field = new ProtoField
        {
            Name = "name",
            Type = "string",
            Number = 1,
            Rule = "optional"
        };
            
        var errors = FieldValidator.ValidateField(field, "proto3").ToList();
        Assert.Single(errors);
        Assert.Contains("Field 'name' does not need the 'optional' label in proto3", errors[0]);
    }
        
    [Fact]
    public void MapWithRule()
    {
        var field = new ProtoField
        {
            Name = "map_field",
            Type = "string",
            Number = 1,
            IsMap = true,
            KeyType = "string",
            Rule = "repeated"
        };
            
        var errors = FieldValidator.ValidateField(field, "proto3").ToList();
        Assert.Single(errors);
        Assert.Contains("Map field 'map_field' cannot have a label", errors[0]);
    }
        
    [Fact]
    public void MapWithEmptyKeyType()
    {
        var field = new ProtoField
        {
            Name = "map_field",
            Type = "string",
            Number = 1,
            IsMap = true,
            KeyType = ""
        };
            
        var errors = FieldValidator.ValidateField(field, "proto3").ToList();
        Assert.Single(errors);
        Assert.Contains("Key type for map field 'map_field' cannot be empty", errors[0]);
    }
        
    [Fact]
    public void MapWithInvalidKeyType()
    {
        var field = new ProtoField
        {
            Name = "map_field",
            Type = "string",
            Number = 1,
            IsMap = true,
            KeyType = "float"
        };
            
        var errors = FieldValidator.ValidateField(field, "proto3").ToList();
        Assert.Single(errors);
        Assert.Contains("Invalid key type 'float' for map field 'map_field'", errors[0]);
    }
}