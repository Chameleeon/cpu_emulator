/** <summary>
 * Interface for the memory management unit. Used for virtual addressing and paging.
 * The user should also implement paging which should be handled in the MMU as well.
 * The user should define the page size and physical memory size.
 * </summary>
 * 
 */
public interface IMMU
{

    /** <summary>
     * Maps virtual address to physical address. Used for paging.
     * Should also implement paging.
     * </summary>
     * <param name = "address">Virtual address</param>
     */
    long mapToPhysical(long address);
}
