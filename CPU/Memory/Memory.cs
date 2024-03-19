#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
/**
 * <summary>
 * Emulates physical memory (RAM).
 * Enables basic features such as reading and writing to memory;
 * </summary>
 */
public class Memory : IMemSpace
{

    public byte ReadByte(long address)
    {
        return 0;
    }
    public void WriteByte(long address, byte value) { }

    public short ReadShort(long address)
    {
        return 0;
    }
    public void WriteShort(long address, short value)
    {

    }

    public int ReadInt(long address)
    {
        return 0;
    }
    public void WriteInt(long address, int value)
    {

    }

    public long ReadLong(long address)
    {
        return 0;
    }
    public void WriteLong(long address, long value)
    {

    }

    public byte[] ReadBytes(long address, int length)
    {
        return null;
    }
    public void WriteBytes(long address, byte[] value)
    {

    }
}
#pragma warning restore CS1591 // Restore XML comment warning
