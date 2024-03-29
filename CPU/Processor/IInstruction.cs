/**
 * <summary>
 * Defines addressing modes for instructions
 * </summary>
 */
public enum AddressingMode
{
    REGISTER = 0x00,
    MEMORY = 0x01,
    IMMEDIATE = 0x02,
    RELATIVE = 0x03
}
/**
 * <summary>
 * IInstruction interface.
 * The IInstruction interface provides a common interface for all instructions.
 * </summary>
 */
public interface IInstruction
{
    /**
     * <summary>
     * The opcode of the instruction
     * </summary>
     */
    byte Opcode { get; set; }
    /**
     * <summary>
     * The addressing mode of the instruction
     * </summary>
     */
    byte AddressingMode { get; set; }
    /**
     * <summary>
     * First operand. If the addressing mode is memory, it contains the address of the operand
     * If the addressing mode is register, it contains the register number
     * </summary>
     */
    long Operand1 { get; set; }
    /**
     * <summary>
     * Second operand. If the addressing mode is memory, it contains the address of the operand
     * If the addressing mode is register, it contains the register number
     * </summary>
     */
    long Operand2 { get; set; }
}
