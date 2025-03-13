namespace Protobuf.Core.Interfaces;

/// <summary>
/// Interface for reading Protocol Buffer encoded data
/// </summary>
public interface ICodedInputStream
{
    bool ReadBool();
    int ReadInt32();
    long ReadInt64();
    uint ReadUInt32();
    ulong ReadUInt64();
    float ReadFloat();
    double ReadDouble();
    string ReadString();
    byte[] ReadBytes();
    uint ReadFixed32();
    ulong ReadFixed64();
    int ReadSFixed32();
    long ReadSFixed64();
    int ReadSInt32();
    long ReadSInt64();
    uint ReadTag();
    bool SkipField(uint tag);
    bool IsAtEnd();
    
    int Position { get; }
    void SetPosition(int position);
}