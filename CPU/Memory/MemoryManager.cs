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
                _instance = new MemoryManager();
            }
            return _instance;
        }
    }

    /** <summary>
     * Memory page size in bytes
     * Default value is 4kB
     * </summary>
     */
    private const int PageSize = 4096;
    /**<summary>
     * Physical memory size in pages
     * Default value is 8GB of memory
     * </summary>
     */
    private const int PhysicalMemorySize = 2048;

    /**
     * <summary>
     * Number of bits used as the page offset
     * </summary>
     */
    private const int pageOffsetBits = 12;

    private MemoryManager()
    {
    }

    /**
     * <summary>
     * Maps virtual address to physical using paging
     * </summary>
     */
    public long mapToPhysical(long virtualAddress)
    {
        return 0;
    }
}
#pragma warning restore CS1591 // Restore XML comment warning
