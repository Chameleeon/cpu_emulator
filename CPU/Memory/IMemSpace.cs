/** <summary>
 * Interface for memory space. Each class implementing this interface should implement the following functionalities:
 * <list type="bullet">
 * <item>Read and Write Byte</item>
 * <item>Read and Write Short</item>
 * <item>Read and Write Int</item>
 * <item>Read and Write Long</item>
 * <item>Read and Write Bytes</item>
 * </list>
 * </summary>
 */
public interface IMemSpace
{
    // int TotalSpace { get; set; }

    /**
     * <summary>
     * Reads byte from memory.
     * </summary>
     * <returns>byte</returns>
     * <param name="address">Memory address</param>
     */
    byte ReadByte(long address);
    /**
     * <summary>
     * Writes byte to memory.
     * </summary>
     * <param name="address">Memory address</param>
     * <param name="value">Byte value</param>
     */
    void WriteByte(long address, byte value);

    /**
     * <summary>
     * Reads short from memory.
     * </summary>
     * <returns>short</returns>
     * <param name="address">Memory address</param>
     */
    short ReadShort(long address);
    /**
     * <summary>
     * Writes short to memory.
     * </summary>
     * <param name="address">Memory address</param>
     * <param name="value">Short value</param>
     */
    void WriteShort(long address, short value);

    /**
     * <summary>
     * Reads int from memory.
     * </summary>
     * <returns>int</returns>
     * <param name="address">Memory address</param>
     */
    int ReadInt(long address);
    /**
     * <summary>
     * Writes int to memory.
     * </summary>
     * <param name="address">Memory address</param>
     * <param name="value">Int value</param>
     */
    void WriteInt(long address, int value);

    /**
     * <summary>
     * Reads long from memory.
     * </summary>
     * <returns>long</returns>
     * <param name="address">Memory address</param>
     */
    long ReadLong(long address);
    /**
     * <summary>
     * Writes long to memory.
     * </summary>
     * <param name="address">Memory address</param>
     * <param name="value">Long value</param>
     */
    void WriteLong(long address, long value);

    /**
     * <summary>
     * Reads bytes from memory.
     * </summary>
     * <returns>byte[]</returns>
     * <param name="address">Memory address</param>
     * <param name="length">Number of bytes to read</param>
     */
    byte[] ReadBytes(long address, int length);
    /**
     * <summary>
     * Writes bytes to memory.
     * </summary>
     * <param name="address">Memory address</param>
     * <param name="value">Byte array</param>
     */
    void WriteBytes(long address, byte[] value);
}

