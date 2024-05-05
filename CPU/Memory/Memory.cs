#pragma warning disable CS1591
/**
 * <summary>
 * Emulates physical memory (RAM).
 * Enables basic features such as reading and writing to memory;
 * </summary>
 */
public class Memory : IMemSpace
{
    public Memory(int pageSize = 4096)
    {
        _pageSize = pageSize;
    }
    /** <summary>
     * Memory page size in bytes
     * Default value is 4kB </summary>
     */
    private readonly int _pageSize;

    /** <summary>
     * Memory space size in bits
     * </summary>
     */
    private const int _memorySpaceSize = 64;

    /**
     * <summary>
     * Number of bits used as the page offset
     * </summary>
     */
    private readonly int _pageOffsetBits;

    /**
     * <summary>
     * Number of bits used as the page number
     * </summary>
     */
    private readonly int _pageNumberBits;

    private Dictionary<long, byte[]> _pageTable = new Dictionary<long, byte[]>();

    /**
     * <summary>
     * Used for paging.
     * </summary>
     * <param name="virtualAddress">Virtual address</param>
     * <returns>The memory page for the given address</returns>
     */
    public byte[] getPage(long virtualAddress)
    {
        long pageNumber = (virtualAddress >> _pageOffsetBits) & ((1 << _pageNumberBits) - 1);
        if (_pageTable.ContainsKey(pageNumber) == false)
        {
            _pageTable[pageNumber] = new byte[_pageSize];
        }
        return _pageTable[pageNumber];
    }

    public byte ReadByte(long address)
    {
        long pageOffset = address & ((1 << _pageOffsetBits) - 1);
        return getPage(address)[pageOffset];
    }
    public void WriteByte(long address, byte value)
    {
        long pageOffset = address & ((1 << _pageOffsetBits) - 1);
        getPage(address)[pageOffset] = value;
    }

    public short ReadShort(long address)
    {
        byte[] page = getPage(address);
        long pageOffset = address & ((1 << _pageOffsetBits) - 1);
        short higherByte = page[pageOffset];
        short lowerByte = page[pageOffset + 1];
        return (short)((higherByte << 8) | lowerByte);
    }
    public void WriteShort(long address, short value)
    {
        byte[] page = getPage(address);
        long pageOffset = address & ((1 << _pageOffsetBits) - 1);
        page[pageOffset] = (byte)(value >> 8);
        page[pageOffset + 1] = (byte)value;
    }

    public int ReadInt(long address)
    {
        short higherBytes = ReadShort(address);
        short lowerBytes = ReadShort(address + 2);
        return (higherBytes << 16) | (lowerBytes & 0xFFFF);
    }
    public void WriteInt(long address, int value)
    {
        short higehrBytes = (short)(value >> 16);
        short lowerBytes = (short)value;
        WriteShort(address, higehrBytes);
        WriteShort(address + 2, lowerBytes);
    }

    public long ReadLong(long address)
    {
        long value = 0;
        byte[] page = getPage(address);
        long pageOffset = address & ((1 << _pageOffsetBits) - 1);

        for (int i = 0; i < 8; i++)
        {
            value = (value << 8) | page[address + i];
        }
        return value;
    }
    public void WriteLong(long address, long value)
    {
        byte[] page = getPage(address);
        long pageOffset = address & ((1 << _pageOffsetBits) - 1);

        for (int i = 7; i >= 0; i--)
        {
            page[pageOffset + i] = (byte)(value & 0xFF);
            value = value >> 8;
        }
    }

    public byte[] ReadBytes(long address, int length)
    {
        byte[] page = getPage(address);
        long pageOffset = address & ((1 << _pageOffsetBits) - 1);

        byte[] array = new byte[length];
        for (int i = 0; i < length; i++)
        {
            array[i] = page[pageOffset + i];
        }
        return array;
    }

    public void WriteBytes(long address, byte[] value)
    {
        byte[] page = getPage(address);
        long pageOffset = address & ((1 << _pageOffsetBits) - 1);
        for (int i = 0; i < value.Length; i++)
        {
            page[pageOffset + i] = value[i];
        }
    }
}
