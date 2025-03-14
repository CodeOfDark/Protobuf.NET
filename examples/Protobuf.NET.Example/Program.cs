using System.Dynamic;
using Protobuf.NET.Extensions;

namespace Protobuf.NET.Example;

internal static class Program
{
    public static void Main(string[] args)
    {
        Console.WriteLine("Person with Message Builder");
        Console.WriteLine("=========================================");
        var serializedPersonWithMessageBuilder = SerializePersonWithMessageBuilder();
        DeserializePerson(serializedPersonWithMessageBuilder);
        
        Console.WriteLine("\nPerson with Expando Object Builder");
        Console.WriteLine("=========================================");
        var serializedPersonWithExpandoObject = SerializePersonWithExpandoObject();
        DeserializePerson(serializedPersonWithExpandoObject);
        
        Console.WriteLine("\nProduct with Message Builder");
        Console.WriteLine("=========================================");
        var serializedProductWithMessageBuilder = SerializeProductWithMessageBuilder();
        DeserializeProduct(serializedProductWithMessageBuilder);
        
        Console.WriteLine("\nProduct with Expando Object Builder");
        Console.WriteLine("=========================================");
        var serializedProductWithExpandoObject = SerializeProductWithExpandoObject();
        DeserializeProduct(serializedProductWithExpandoObject);
    }
    
    private static byte[] SerializePersonWithExpandoObject()
    {
        dynamic homeNumber = new ExpandoObject();
        homeNumber.number = (1, "+1234567890");
        homeNumber.type = (2, 1);

        dynamic workNumber = new ExpandoObject();
        workNumber.number = (1, "+1234567890");
        workNumber.type = (2, 2);
        
        dynamic person = new ExpandoObject();
        person.name = (1, "Protobuf.NET");
        person.id = (2, 1);
        person.email = (3, "example@example.com");
        person.phones = (4, new List<dynamic> { homeNumber, workNumber });
        person.last_updated = (5, new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds());
        person.email_pref = (6, "example@example.com");
        person.nickname = (101, "Example");

        dynamic addressBook = new ExpandoObject();
        addressBook.people = (1, new List<dynamic> { person });
        
        return ProtobufSerializer.Serialize(addressBook);
    }
    
    private static byte[] SerializeProductWithExpandoObject()
    {
        dynamic product = new ExpandoObject();
        product.product_name = (1, "T-Shirt");
        product.available_quantities = (2, 1000);
        product.images = (3, new List<string> { "https://example.com/image1.png", "https://example.com/image2.png" });
        
        return ProtobufSerializer.Serialize(product);
    }

    private static byte[] SerializePersonWithMessageBuilder()
    {
        var person = ExpandoObjectExtensions.CreateMessage()
            .AddField("name", 1, "Protobuf.NET")
            .AddField("id", 2, 1)
            .AddField("email", 3, "example@example.com")
            .AddField("last_updated", 5, new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds())
            .AddField("email_pref", 6, "example@example.com")
            .AddField("nickname", 101, "Example");

        var phoneNumbers = new List<dynamic>()
        {
            ExpandoObjectExtensions.CreateMessage()
                .AddField("number", 1, "+1234567890")
                .AddField("type", 2, 1).Build(),
            ExpandoObjectExtensions.CreateMessage()
                .AddField("number", 1, "+1234567890")
                .AddField("type", 2, 2).Build()
        };
        
        person.AddField("phones", 4, phoneNumbers);

        var addressBook = ExpandoObjectExtensions.CreateMessage()
            .AddField("people", 1, new List<dynamic> { person.Build() });
        
        return addressBook.BuildAndSerialize();
    }

    private static byte[] SerializeProductWithMessageBuilder()
    {
        var product = ExpandoObjectExtensions.CreateMessage()
            .AddField("product_name", 1, "T-Shirt")
            .AddField("available_quantities", 2, 1000)
            .AddField("images", 3, new[] { "https://example.com/image1.png", "https://example.com/image2.png" });

        return product.BuildAndSerialize();
    }

    private static void DeserializePerson(byte[] serializedPerson)
    {
        dynamic deserialized = serializedPerson.DeserializeWithDirectorySchema("./../../../../../samples/person", "example.person.AddressBook");
        foreach (var people in deserialized.people)
        {
            Console.WriteLine($"Id: {people.id}");
            Console.WriteLine($"Name: {people.name}");
            Console.WriteLine($"Nickname: {people.nickname}");
            Console.WriteLine($"Email: {people.email}");
            Console.WriteLine($"Email Pref: {people.email_pref}");
            Console.WriteLine($"Last Updated: {people.last_updated}");
            Console.WriteLine($"PhoneNumbers:");
            foreach (var phone in people.phones)
            {
                Console.WriteLine($" - Phone: {phone.number}");
                Console.WriteLine($" - Type: {(phone.type == 1 ? "Home" : "Work")}");
            }
        }
    }

    private static void DeserializeProduct(byte[] serializedProduct)
    {
        dynamic deserialized = serializedProduct.DeserializeWithSchema("./../../../../../samples/product/product.proto", "Product");
        Console.WriteLine($"Product Name: {deserialized.product_name}");
        Console.WriteLine($"Available Quantities: {deserialized.available_quantities}");
        Console.WriteLine($"Images:");
        foreach (var image in deserialized.images)
            Console.WriteLine($" - Image: {image}");
    }
}