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
    int _programCounter { get; set; }

    /**
     * <summary>
     * Executes an instruction.
     * </summary>
     */
    void ExecuteInstruction();
    /**
     * <summary>
     * Loads instructions from a file into memory
     * </summary>
     * <param name="filePath">Input file path</param>
     */
    void ParseInstructions(string filePath);
}

