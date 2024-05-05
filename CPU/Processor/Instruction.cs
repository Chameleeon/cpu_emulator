/**
 * <summary>
 * IInstruction interface.
 * The IInstruction interface provides a common interface for all instructions.
 * </summary>
 */
public class Instruction
{
    /**
     * <summary>
     * The opcode of the instruction
     * </summary>
     */
    public byte Opcode { get; set; }
    /**
     * <summary>
     * The MODRR byte, check the config/InstructionSet.cnf file for more details
     * </summary>
     */
    public byte MODRR { get; set; }
    /**
     * <summary>
     * Memory address being used in the instruction. Optional field if two registers or a register and immediate value are used with 2 operand instructions.
     * </summary>
     */
    public long MemoryAddress { get; set; }
    /**
     * <summary>
     * Immediate value being used in the instruction. Optional field if two regiters or a register and memory value are used wit 2 operand instructions.
     * </summary>
     */
    public long ImmediateValue { get; set; }

    public Instruction()
    {
        Opcode = 0;
        MODRR = 0;
        MemoryAddress = 0;
        ImmediateValue = 0;
    }
}
