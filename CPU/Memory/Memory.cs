#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
/**
 * <summary>
 * Emulates physical memory (RAM).
 * Enables basic features such as reading and writing to memory;
 * </summary>
 */
public class Memory : IMemSpace
{
    private readonly int _memorySize;
    private readonly int _pageSize;
    private bool[] _freePages;
    private Utils.LongByteArray _memory;

    // Convert into singleton
    private static Memory _instance;
    public static Memory Instance
    {
        get
        {
            if (_instance == null)
            {
                throw new InvalidOperationException("Singleton instance has not been initialized.");
            }
            return _instance;
        }
    }
    public static Memory Initialize(int memorySize = 2048, int pageSize = 4096)
    {
        if (_instance == null)
        {
            _instance = new Memory(memorySize, pageSize);
            return _instance;
        }
        else
        {
            throw new InvalidOperationException("Singleton instance has already been initialized.");
        }
    }

    private Memory(int memorySize, int pageSize = 4096)
    {
        _memorySize = memorySize;
        _pageSize = pageSize;
        _freePages = new bool[memorySize];
        for (int i = 0; i < _memorySize; i++)
        {
            _freePages[i] = true;
        }
        _memory = new Utils.LongByteArray(memorySize * pageSize);
    }

    public byte ReadByte(long address)
    {
        return _memory[address];
    }
    public void WriteByte(long address, byte value)
    {
        _memory[address] = value;
    }

    public short ReadShort(long address)
    {
        short higherByte = _memory[address];
        short lowerByte = _memory[address + 1];
        return (short)((higherByte << 8) | lowerByte);
    }
    public void WriteShort(long address, short value)
    {
        _memory[address] = (byte)(value >> 8);
        _memory[address + 1] = (byte)(value);
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

        for (int i = 0; i < 8; i++)
        {
            value = (value << 8) | _memory[address + i];
        }
        return value;
    }
    public void WriteLong(long address, long value)
    {
        for (int i = 7; i >= 0; i--)
        {
            _memory[address + i] = (byte)(value & 0xFF);
            value = value >> 8;
        }
    }

    public byte[] ReadBytes(long address, int length)
    {
        byte[] array = new byte[length];
        for (int i = 0; i < length; i++)
        {
            array[i] = _memory[address + i];
        }
        return array;
    }

    public void WriteBytes(long address, byte[] value)
    {
        for (int i = 0; i < value.Length; i++)
        {
            _memory[address + i] = value[i];
        }
    }

    // TODO use the virtual address to create a list of references to the physical block
    public long AllocatePhysicalMemBlock(int virtualAddress)
    {
        for (int i = 0; i < _freePages.Length; i++)
        {
            if (_freePages[i])
            {
                _freePages[i] = false;
                return i * _pageSize;
            }
        }
        throw new OutOfMemoryException("No free pages available.");
    }

    // TODO keep references of allocated memory block so that if more than one virtual memory references it,
    // the actual physical block is marked as free only once there are no more references, otherwise just remove
    // the reference
    public void FreePhysicalMemBlock(long address)  // ASSUMES THAT THE ADDRESS RECEIVED IS THE ADDRESS OF A PHYSICAL BLOCK
    {
        _freePages[address] = true;
    }
}
