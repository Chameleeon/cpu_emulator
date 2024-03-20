#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
/**
 * <summary>
 * Singleton class for the memory management unit. Used for virtual addressing and paging.
 * </summary>
 */
public class MemoryManager : IMMU
{
    /**<summary>
     * Singleton instance
     * </summary>
     */
    private static MemoryManager? _instance = null;
    /** <summary>
     * Singleton instance interface
     * </summary>
     */
    public static MemoryManager Instance
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

    /** <summary>
     * Initializes the singleton instance
     * </summary>
     * <param name="memorySize">Physical memory size in pages.</param>
     * <param name="pageSize">Memory page size in bytes.</param>
     */
    public static MemoryManager Initialize(int memorySize, int pageSize)
    {
        if (_instance == null)
        {
            _instance = new MemoryManager(memorySize, pageSize);
            return _instance;
        }
        else
        {
            throw new InvalidOperationException("Singleton instance has already been initialized.");
        }
    }

    /** <summary>
     * Memory page size in bytes
     * Default value is 4kB
     * </summary>
     */
    private const int _memorySpaceSize = 64; // Memory space size in bits

    private IMemSpace _memory;

    private readonly int _pageSize;
    /**<summary>
     * Physical memory size in pages
     * Default value is 8GB of memory which equals to 2048 pages
     * </summary>
     */
    private readonly int _physicalMemorySize;

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
    private Dictionary<long, long> _pageTable = new Dictionary<long, long>();

    private MemoryManager(int physicalMemorySize = 2048, int pageSize = 4096)
    {
        _physicalMemorySize = physicalMemorySize;
        _pageSize = pageSize;
        _memory = new Memory(_physicalMemorySize, _pageSize);      // create physical memory
        _pageOffsetBits = (int)Math.Log2(_pageSize);
        _pageNumberBits = _memorySpaceSize - _pageOffsetBits;
        AllocateMemory();
    }

    private void AllocateMemory()
    {
        for (int i = _physicalMemorySize - 1; i >= 0; i--)
        {
            _pageTable.Add(i, _memory.AllocatePhysicalMemBlock(i));
        }
    }

    /**
     * <summary>
     * Maps virtual address to physical using paging
     * </summary>
     * <param name="virtualAddress">Virtual address</param>
     * <returns>Physical address if the virtual address is mapped, -1 otherwise</returns>
     */
    public long TranslateAddress(long virtualAddress)
    {
        long pageOffset = virtualAddress & ((1 << _pageOffsetBits) - 1);
        long pageNumber = (virtualAddress >> _pageOffsetBits) & ((1 << _pageNumberBits) - 1);
        if (_pageTable.ContainsKey(pageNumber) == false)
        {
            return -1;
        }
        long physicalAddress = _pageTable[pageNumber] + pageOffset;
        return physicalAddress;
    }

}
#pragma warning restore CS1591 // Restore XML comment warning
