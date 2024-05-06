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
    public bool HasImmediate { get; set; }
    public bool HasAddress { get; set; }

    public Instruction()
    {
        Opcode = 0;
        MODRR = 0;
        MemoryAddress = 0;
        ImmediateValue = 0;
    }

    public override string ToString()
    {
        return "OPCODE: " + Opcode + "\nMODRR: " + MODRR + "\nMemoryAddress: " + MemoryAddress + "\nImmedaite: " + ImmediateValue;
    }

    public byte[] ToByteArray()
    {
        byte[] bytes;
        if (HasImmediate)
        {
            bytes = new byte[10];
            bytes[2] = (byte)(ImmediateValue >> 56);
            bytes[3] = (byte)(ImmediateValue >> 48);
            bytes[4] = (byte)(ImmediateValue >> 40);
            bytes[5] = (byte)(ImmediateValue >> 32);
            bytes[6] = (byte)(ImmediateValue >> 24);
            bytes[7] = (byte)(ImmediateValue >> 16);
            bytes[8] = (byte)(ImmediateValue >> 8);
            bytes[9] = (byte)ImmediateValue;
        }
        else if (HasAddress)
        {
            bytes = new byte[10];
            bytes[2] = (byte)(MemoryAddress >> 56);
            bytes[3] = (byte)(MemoryAddress >> 48);
            bytes[4] = (byte)(MemoryAddress >> 40);
            bytes[5] = (byte)(MemoryAddress >> 32);
            bytes[6] = (byte)(MemoryAddress >> 24);
            bytes[7] = (byte)(MemoryAddress >> 16);
            bytes[8] = (byte)(MemoryAddress >> 8);
            bytes[9] = (byte)MemoryAddress;
        }
        else
        {
            bytes = new byte[2];
        }
        bytes[0] = Opcode;
        bytes[1] = (byte)MODRR;
        return bytes;
    }
}
