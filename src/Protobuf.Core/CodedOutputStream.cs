using System.Text;
using Protobuf.Core.Enums;
using Protobuf.Core.Interfaces;

namespace Protobuf.Core;

/// <summary>
/// Implementation of Protocol Buffers binary encoding for writing messages
/// </summary>
public class CodedOutputStream : ICodedOutputStream
{
    private readonly List<byte> _buffer;
    
    public int Position => _buffer.Count;

    public CodedOutputStream()
    {
        _buffer = [];
    }

    public CodedOutputStream(int capacity)
    {
        _buffer = new List<byte>(capacity);
    }

    public void WriteBool(bool value)
    {
        WriteRawByte((byte)(value ? 1 : 0));
    }

    public void WriteInt32(int value)
    {
        if (value >= 0)
        {
            WriteRawVarint32((uint)value);
        }
        else
        {
            // ReSharper disable once IntVariableOverflowInUncheckedContext
            WriteRawVarint64((ulong)value);
        }
    }

    public void WriteInt64(long value)
    {
        WriteRawVarint64((ulong)value);
    }

    public void WriteUInt32(uint value)
    {
        WriteRawVarint32(value);
    }

    public void WriteUInt64(ulong value)
    {
        WriteRawVarint64(value);
    }

    public void WriteFloat(float value)
    {
        WriteRawLittleEndian32(BitConverter.ToUInt32(BitConverter.GetBytes(value), 0));
    }

    public void WriteDouble(double value)
    {
        WriteRawLittleEndian64(BitConverter.ToUInt64(BitConverter.GetBytes(value), 0));
    }

    public void WriteString(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            WriteRawVarint32(0);
            return;
        }

        byte[] bytes = Encoding.UTF8.GetBytes(value);
        WriteRawVarint32((uint)bytes.Length);
        WriteRawBytes(bytes);
    }

    public void WriteBytes(byte[] value)
    {
        if (value.Length == 0)
        {
            WriteRawVarint32(0);
            return;
        }

        WriteRawVarint32((uint)value.Length);
        WriteRawBytes(value);
    }

    public void WriteFixed32(uint value)
    {
        WriteRawLittleEndian32(value);
    }

    public void WriteFixed64(ulong value)
    {
        WriteRawLittleEndian64(value);
    }

    public void WriteSFixed32(int value)
    {
        WriteRawLittleEndian32((uint)value);
    }

    public void WriteSFixed64(long value)
    {
        WriteRawLittleEndian64((ulong)value);
    }

    public void WriteSInt32(int value)
    {
        WriteRawVarint32((uint)((value << 1) ^ (value >> 31)));
    }

    public void WriteSInt64(long value)
    {
        WriteRawVarint64((ulong)((value << 1) ^ (value >> 63)));
    }

    public void WriteTag(int fieldNumber, WireFormat wireType)
    {
        WriteRawVarint32(WireFormatUtility.MakeTag(fieldNumber, wireType));
    }

    public byte[] ToByteArray()
    {
        return _buffer.ToArray();
    }
    
    private void WriteRawByte(byte value)
    {
        _buffer.Add(value);
    }

    private void WriteRawBytes(byte[] value)
    {
        _buffer.AddRange(value);
    }

    private void WriteRawVarint32(uint value)
    {
        while (value > 0x7F)
        {
            WriteRawByte((byte)((value & 0x7F) | 0x80));
            value >>= 7;
        }
        WriteRawByte((byte)value);
    }

    private void WriteRawVarint64(ulong value)
    {
        while (value > 0x7F)
        {
            WriteRawByte((byte)((value & 0x7F) | 0x80));
            value >>= 7;
        }
        WriteRawByte((byte)value);
    }

    private void WriteRawLittleEndian32(uint value)
    {
        WriteRawByte((byte)(value & 0xFF));
        WriteRawByte((byte)((value >> 8) & 0xFF));
        WriteRawByte((byte)((value >> 16) & 0xFF));
        WriteRawByte((byte)((value >> 24) & 0xFF));
    }

    private void WriteRawLittleEndian64(ulong value)
    {
        WriteRawByte((byte)(value & 0xFF));
        WriteRawByte((byte)((value >> 8) & 0xFF));
        WriteRawByte((byte)((value >> 16) & 0xFF));
        WriteRawByte((byte)((value >> 24) & 0xFF));
        WriteRawByte((byte)((value >> 32) & 0xFF));
        WriteRawByte((byte)((value >> 40) & 0xFF));
        WriteRawByte((byte)((value >> 48) & 0xFF));
        WriteRawByte((byte)((value >> 56) & 0xFF));
    }
}