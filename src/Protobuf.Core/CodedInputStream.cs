using Protobuf.Core.Enums;
using Protobuf.Core.Interfaces;

namespace Protobuf.Core;

/// <summary>
/// Implementation of Protocol Buffers binary decoding for reading messages
/// </summary>
public class CodedInputStream(byte[] buffer) : ICodedInputStream
{
    public int Position { get; private set; }

    public bool ReadBool()
    {
        return ReadRawByte() != 0;
    }

    public int ReadInt32()
    {
        return (int)ReadRawVarint32();
    }

    public long ReadInt64()
    {
        return (long)ReadRawVarint64();
    }

    public uint ReadUInt32()
    {
        return ReadRawVarint32();
    }

    public ulong ReadUInt64()
    {
        return ReadRawVarint64();
    }

    public float ReadFloat()
    {
        return BitConverter.ToSingle(BitConverter.GetBytes(ReadRawLittleEndian32()), 0);
    }

    public double ReadDouble()
    {
        return BitConverter.ToDouble(BitConverter.GetBytes(ReadRawLittleEndian64()), 0);
    }

    public string ReadString()
    {
        var length = (int)ReadRawVarint32();
        return length == 0 ? string.Empty : System.Text.Encoding.UTF8.GetString(ReadRawBytes(length));
    }

    public byte[] ReadBytes()
    {
        var length = (int)ReadRawVarint32();
        return length == 0 ? [] : ReadRawBytes(length);
    }

    public uint ReadFixed32()
    {
        return ReadRawLittleEndian32();
    }

    public ulong ReadFixed64()
    {
        return ReadRawLittleEndian64();
    }

    public int ReadSFixed32()
    {
        return (int)ReadRawLittleEndian32();
    }

    public long ReadSFixed64()
    {
        return (long)ReadRawLittleEndian64();
    }

    public int ReadSInt32()
    {
        var value = ReadRawVarint32();
        return (int)((value >> 1) ^ -(value & 1));
    }

    public long ReadSInt64()
    {
        var value = ReadRawVarint64();
        return (long)((value >> 1) ^ (0 - (value & 1)));
    }

    public uint ReadTag()
    {
        return IsAtEnd() ? 0 : ReadRawVarint32();
    }

    public bool SkipField(uint tag)
    {
        var wireType = WireFormatUtility.GetWireTypeFromTag(tag);

        switch (wireType)
        {
            case WireFormat.Varint:
                ReadRawVarint64();
                return true;
            case WireFormat.Fixed64:
                ReadRawLittleEndian64();
                return true;
            case WireFormat.LengthDelimited:
                ReadBytes();
                return true;
            case WireFormat.Fixed32:
                ReadRawLittleEndian32();
                return true;
            default:
                return false;
        }
    }

    public bool IsAtEnd()
    {
        return Position >= buffer.Length;
    }
    
    public void SetPosition(int position)
    {
        Position = position;
    }

    private byte ReadRawByte()
    {
        if (IsAtEnd())
            throw new EndOfStreamException();
        
        return buffer[Position++];
    }

    private byte[] ReadRawBytes(int count)
    {
        var result = new byte[count];
        
        Array.Copy(buffer, Position, result, 0, count);
        Position += count;
        
        return result;
    }
    
    private uint ReadRawVarint32()
    {
        if (buffer.Length > Position + 5) 
            return (uint)ReadRawVarint64();
            
        uint result = 0; 
        var shift = 0;

        while (shift <= 35)
        {
            var b = buffer[Position++];
            result |= (uint)(b & 0x7F) << shift;
            shift += 7;

            if ((b & 0x80) == 0)
                break;
        }

        return result;
    }
    
    private ulong ReadRawVarint64()
    {
        ulong result = 0; 
        var shift = 0;

        while (shift <= 70)
        {
            var b = buffer[Position++];
            result |= (ulong)(b & 0x7F) << shift;
            shift += 7;

            if ((b & 0x80) == 0)
                break;
        }

        return result;
    }

    private uint ReadRawLittleEndian32()
    {
        uint result = ReadRawByte();
        result |= (uint)ReadRawByte() << 8;
        result |= (uint)ReadRawByte() << 16;
        result |= (uint)ReadRawByte() << 24;
        return result;
    }

    private ulong ReadRawLittleEndian64()
    {
        ulong result = ReadRawByte();
        result |= (ulong)ReadRawByte() << 8;
        result |= (ulong)ReadRawByte() << 16;
        result |= (ulong)ReadRawByte() << 24;
        result |= (ulong)ReadRawByte() << 32;
        result |= (ulong)ReadRawByte() << 40;
        result |= (ulong)ReadRawByte() << 48;
        result |= (ulong)ReadRawByte() << 56;
        return result;
    }
}