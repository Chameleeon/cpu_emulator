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
    private byte[] _memory;

    public Memory(int memorySize, int pageSize = 4096)
    {
        _memorySize = memorySize;
        _pageSize = pageSize;
        _freePages = new bool[memorySize];
        for (int i = 0; i < _memorySize; i++)
        {
            _freePages[i] = true;
        }
        _memory = new byte[memorySize * pageSize];
    }

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
#pragma warning restore CS1591 // Restore XML comment warning
