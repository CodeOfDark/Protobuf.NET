using Protobuf.Core.Enums;

namespace Protobuf.Core.Interfaces;

/// <summary>
/// Interface for writing Protocol Buffer encoded data
/// </summary>
public interface ICodedOutputStream
{
    void WriteBool(bool value);
    void WriteInt32(int value);
    void WriteInt64(long value);
    void WriteUInt32(uint value);
    void WriteUInt64(ulong value);
    void WriteFloat(float value);
    void WriteDouble(double value);
    void WriteString(string value);
    void WriteBytes(byte[] value);
    void WriteFixed32(uint value);
    void WriteFixed64(ulong value);
    void WriteSFixed32(int value);
    void WriteSFixed64(long value);
    void WriteSInt32(int value);
    void WriteSInt64(long value);
    void WriteTag(int fieldNumber, WireFormat wireType);
    
    int Position { get; }
    byte[] ToByteArray();
}