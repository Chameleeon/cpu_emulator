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
    IRegister[] Registers { get; set; }

    /**
     * <summary>
     * The memory manager of the processor.
     * </summary>
     */
    IMMU MemoryManager { get; set; }

    /**
     * <summary>
     * The program counter of the processor.
     * </summary>
     */
    int ProgramCounter { get; set; }

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

