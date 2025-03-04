using Protobuf.Core.Enums;

namespace Protobuf.Core.Tests;

public class CodedInputStreamTest
{
    [Theory]
    [InlineData(new byte[] { 0x00 }, false)]
    [InlineData(new byte[] { 0x01 }, true)]
    public void ReadBool(byte[] buffer, bool expected)
    {
        var codedInputStream = new CodedInputStream(buffer);
        Assert.Equal(expected, codedInputStream.ReadBool());
    }
    
    [Theory]
    [InlineData(new byte[] { 0xb9, 0x0a }, 1337)]
    [InlineData(new byte[] { 0xff, 0xff, 0xff, 0xff, 0x07 }, int.MaxValue)]
    [InlineData(new byte[] { 0xc7, 0xf5, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0x01 }, -1337)]
    [InlineData(new byte[] { 0x81, 0x80, 0x80, 0x80, 0xf8, 0xff, 0xff, 0xff, 0xff, 0x01 }, -int.MaxValue)]
    public void ReadInt32(byte[] buffer, int expected)
    {
        var codedInputStream = new CodedInputStream(buffer);
        Assert.Equal(expected, codedInputStream.ReadInt32());
    }
    
    [Theory]
    [InlineData(new byte[] { 0xb9, 0x0a }, 1337)]
    [InlineData(new byte[] { 0xff, 0xff, 0xff, 0xff, 0x07 }, int.MaxValue)]
    [InlineData(new byte[] { 0xff, 0xff, 0xff, 0xff, 0x7f }, uint.MaxValue)]
    public void ReadUInt32(byte[] buffer, uint expected)
    {
        var codedInputStream = new CodedInputStream(buffer);
        Assert.Equal(expected, codedInputStream.ReadUInt32());
    }
    
    [Theory]
    [InlineData(new byte[] { 0xf2, 0x14 }, 1337)]
    [InlineData(new byte[] { 0xfe, 0xff, 0xff, 0xff, 0x0f }, int.MaxValue)]
    [InlineData(new byte[] { 0xf1, 0x14 }, -1337)]
    [InlineData(new byte[] { 0xfd, 0xff, 0xff, 0xff, 0x0f }, -int.MaxValue)]
    public void ReadSInt32(byte[] buffer, int expected)
    {
        var codedInputStream = new CodedInputStream(buffer);
        Assert.Equal(expected, codedInputStream.ReadSInt32());
    }
    
    [Theory]
    [InlineData(new byte[] { 0xf2, 0x14 }, 1337)]
    [InlineData(new byte[] { 0xfe, 0xff, 0xff, 0xff, 0x0f }, int.MaxValue)]
    [InlineData(new byte[] { 0xf1, 0x14 }, -1337)]
    [InlineData(new byte[] { 0xfd, 0xff, 0xff, 0xff, 0x0f }, -int.MaxValue)]
    [InlineData(new byte[] { 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0x01 }, long.MinValue)]
    public void ReadSInt64(byte[] buffer, long expected)
    {
        var codedInputStream = new CodedInputStream(buffer);
        Assert.Equal(expected, codedInputStream.ReadSInt64());
    }
    
    [Theory]
    [InlineData(new byte[] { 0x39, 0x05, 0x00, 0x00 }, 1337)]
    [InlineData(new byte[] { 0xff, 0xff, 0xff, 0x7f }, int.MaxValue)]
    [InlineData(new byte[] { 0xc7, 0xfa, 0xff, 0xff }, -1337)]
    [InlineData(new byte[] { 0x01, 0x00, 0x00, 0x80 }, -int.MaxValue)]
    public void ReadSFixed32(byte[] buffer, int expected)
    {
        var codedInputStream = new CodedInputStream(buffer);
        Assert.Equal(expected, codedInputStream.ReadSFixed32());
    }
    
    [Theory]
    [InlineData(new byte[] { 0x39, 0x05, 0x00, 0x00 }, 1337)]
    [InlineData(new byte[] { 0xff, 0xff, 0xff, 0x7f }, int.MaxValue)]
    public void ReadFixed32(byte[] buffer, uint expected)
    {
        var codedInputStream = new CodedInputStream(buffer);
        Assert.Equal(expected, codedInputStream.ReadFixed32());
    }
    
    [Theory]
    [InlineData(new byte[] { 0xb9, 0x0a }, 1337)]
    [InlineData(new byte[] { 0xff, 0xff, 0xff, 0xff, 0x07 }, int.MaxValue)]
    [InlineData(new byte[] { 0xc7, 0xf5, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0x01 }, -1337)]
    [InlineData(new byte[] { 0x81, 0x80, 0x80, 0x80, 0xf8, 0xff, 0xff, 0xff, 0xff, 0x01 }, -int.MaxValue)]
    [InlineData(new byte[] { 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0x7f }, long.MaxValue)]
    [InlineData(new byte[] { 0x81, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x01 }, -long.MaxValue)]
    public void ReadInt64(byte[] buffer, long expected)
    {
        var codedInputStream = new CodedInputStream(buffer);
        Assert.Equal(expected, codedInputStream.ReadInt64());
    }
    
    [Theory]
    [InlineData(new byte[] { 0x39, 0x05, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, 1337)]
    [InlineData(new byte[] { 0xff, 0xff, 0xff, 0x7f, 0x00, 0x00, 0x00, 0x00 }, int.MaxValue)]
    [InlineData(new byte[] { 0xc7, 0xfa, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff }, -1337)]
    [InlineData(new byte[] { 0x01, 0x00, 0x00, 0x80, 0xff, 0xff, 0xff, 0xff }, -int.MaxValue)]
    [InlineData(new byte[] { 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0x7f }, long.MaxValue)]
    [InlineData(new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x80 }, long.MinValue)]
    public void ReadSFixed64(byte[] buffer, long expected)
    {
        var codedInputStream = new CodedInputStream(buffer);
        Assert.Equal(expected, codedInputStream.ReadSFixed64());
    }
    
    [Theory]
    [InlineData(new byte[] { 0x39, 0x05, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, 1337)]
    [InlineData(new byte[] { 0xff, 0xff, 0xff, 0x7f, 0x00, 0x00, 0x00, 0x00 }, int.MaxValue)]
    [InlineData(new byte[] { 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0x7f }, long.MaxValue)]
    public void ReadFixed64(byte[] buffer, long expected)
    {
        var codedInputStream = new CodedInputStream(buffer);
        Assert.Equal(expected, codedInputStream.ReadSFixed64());
    }
    
    [Theory]
    [InlineData(new byte[] { 0xb9, 0x0a }, 1337)]
    [InlineData(new byte[] { 0xff, 0xff, 0xff, 0xff, 0x07 }, int.MaxValue)]
    [InlineData(new byte[] { 0xff, 0xff, 0xff, 0xff, 0x0f }, uint.MaxValue)]
    [InlineData(new byte[] { 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0x7f }, long.MaxValue)]
    [InlineData(new byte[] { 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0x01 }, ulong.MaxValue)]
    public void ReadUInt64(byte[] buffer, ulong expected)
    {
        var codedInputStream = new CodedInputStream(buffer);
        Assert.Equal(expected, codedInputStream.ReadUInt64());
    }
    
    [Theory]
    [InlineData(new byte[] { 0x85, 0xeb, 0x55, 0x41 }, 13.37)]
    [InlineData(new byte[] { 0xff, 0xf0, 0x55, 0x41 }, 13.371337)]
    public void ReadFloat(byte[] buffer, float expected)
    {
        var codedInputStream = new CodedInputStream(buffer);
        Assert.Equal(expected, codedInputStream.ReadFloat());
    }
    
    [Theory]
    [InlineData(new byte[] { 0x3d, 0x0a, 0xd7, 0xa3, 0x70, 0xbd, 0x2a, 0x40 }, 13.37)]
    [InlineData(new byte[] { 0xea, 0x96, 0x1d, 0xe2, 0x1f, 0xbe, 0x2a, 0x40 }, 13.371337)]
    [InlineData(new byte[] { 0x9d, 0x59, 0x93, 0xee, 0x1f, 0xbe, 0x2a, 0x40 }, 13.371337371337)]
    public void ReadDouble(byte[] buffer, double expected)
    {
        var codedInputStream = new CodedInputStream(buffer);
        Assert.Equal(expected, codedInputStream.ReadDouble());
    }
    
    [Theory]
    [InlineData(new byte[] { 0x0c, 0x50, 0x72, 0x6f, 0x74, 0x6f, 0x62, 0x75, 0x66, 0x2e, 0x4e, 0x45, 0x54 }, "Protobuf.NET")]
    public void ReadString(byte[] buffer, string expected)
    {
        var codedInputStream = new CodedInputStream(buffer);
        Assert.Equal(expected, codedInputStream.ReadString());
    }

    [Theory]
    [InlineData(new byte[] { 0xc8, 0x53 }, WireFormat.Varint, 1337)]
    [InlineData(new byte[] { 0xcd, 0x53 }, WireFormat.Fixed32, 1337)]
    [InlineData(new byte[] { 0xc9, 0x53 }, WireFormat.Fixed64, 1337)]
    [InlineData(new byte[] { 0xca, 0x53 }, WireFormat.LengthDelimited, 1337)]
    public void ReadTag(byte[] buffer, WireFormat wireFormat, int fieldNumber)
    {
        var codedInputStream = new CodedInputStream(buffer);
        var tag = codedInputStream.ReadTag();
        var wire = WireFormatUtility.GetWireTypeFromTag(tag);
        var field = WireFormatUtility.GetFieldNumberFromTag(tag);
        
        Assert.Equal(wireFormat, wire);
        Assert.Equal(fieldNumber, field);
    }
}