/**
 * <summary>
 * IProcessor interface.
 * The IProcessor interface provides a common interface for all processors.
 * It contains the registers, the memory manager and the program counter.
 * </summary>
 */
public interface IProcessor
{
    /**
     * <summary>
     * The registers of the processor.
     * </summary>
     */
    IRegister[] _registers { get; set; }

    /**
     * <summary>
     * The memory of the processor
     * </summary>
     */
    IMemSpace _memory { get; set; }

    /**
     * <summary>
     * The program counter of the processor.
     * </summary>
     */
    long _programCounter { get; set; }

    /**
     * <summary>
     * Executes a program provided the input binary file
     * </summary>
     * <param name="filePath">Input file path</param>
     */
    void ExecuteProgram(string filePath);
}

