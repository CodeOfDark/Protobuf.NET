# Protobuf.NET

[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](https://opensource.org/licenses/MIT)

A flexible Protocol Buffers serializer/deserializer for .NET that supports dynamic objects, dictionaries, and schema-aware deserialization.

## Overview

Protobuf.NET provides a complete solution for working with Protocol Buffers in .NET environments. It allows you to:

- Parse and validate `.proto` files
- Serialize .NET objects to Protocol Buffers binary format
- Deserialize Protocol Buffers data in multiple flexible ways
  - As dynamic objects
  - As dictionaries
  - As schema-aware dynamic objects

## To build the project from the command line, use:

```bash
dotnet build
```

## Key Features

- **Proto File Support**: Parser and validator for `.proto` files
- **Flexible Serialization**: Easily serialize data on the fly
- **Multiple Deserialization Options**:
  - Dynamic objects
  - Dictionaries with type information
  - Schema-aware dynamics that use field names from your proto files

## Usage Examples

### Parsing and Validating Proto Files

```csharp
var parser = new ProtoParser();
var validator = new ProtoValidator();

var protoFile = parser.ParseFile("product.proto");
validator.Validate(protoFile);
```

### Serialization

#### Using ExpandoObject

```csharp
dynamic product = new ExpandoObject();
product.product_name = (1, "T-Shirt");
product.available_quantities = (2, 1000);
product.images = (3, new List<string> { "https://example.com/image1.png" });

byte[] serializedData = ProtobufSerializer.Serialize(product);
```

> **Note**: The value must always be a tuple where the first element indicates the field number.

#### Using Extension Message Builder

```csharp
var product = ExpandoObjectExtensions.CreateMessage()
    .AddField("product_name", 1, "T-Shirt")
    .AddField("available_quantities", 2, 1000)
    .AddField("images", 3, new[] { "https://example.com/image1.png" });

byte[] serializedData = product.BuildAndSerialize();
```

### Deserialization

#### As Dictionary

Keys are formatted as `field_number(field_type)` and arrays as `field_number(field_type)[]`

```csharp
var deserialized = serializedProduct.DeserializeAsDictionary();

Console.WriteLine($"Product Name: {deserialized["1(string)"]}");
Console.WriteLine($"Available Quantities: {deserialized["2(int32)"]}");
Console.WriteLine($"Images:");
foreach (var image in (List<object?>)deserialized["3(string)[]"])
    Console.WriteLine($" - Image: {image}");
```

#### As Dynamic

Property names are formatted as `fieldType_fieldNumber_(Array if any)`

```csharp
dynamic deserialized = serializedProduct.DeserializeAsDynamic();

Console.WriteLine($"Product Name: {deserialized.String_1}");
Console.WriteLine($"Available Quantities: {deserialized.Int32_2}");
Console.WriteLine($"Images:");
foreach (var image in (List<object?>)deserialized.String_3_Array)
    Console.WriteLine($" - Image: {image}");
```

#### As Schema-aware Dynamic

Uses original field names from the proto file definition

```csharp
dynamic deserialized = serializedProduct.DeserializeWithSchema("product.proto", "Product");

Console.WriteLine($"Product Name: {deserialized.product_name}");
Console.WriteLine($"Available Quantities: {deserialized.available_quantities}");
Console.WriteLine($"Images:");
foreach (var image in deserialized.images)
    Console.WriteLine($" - Image: {image}");
```

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## License

Protobuf.NET is released under the [MIT license](LICENSE).