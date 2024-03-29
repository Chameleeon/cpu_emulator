public class Instruction : IInstruction
{
    public byte Opcode { get; set; }
    public byte AddressingMode { get; set; }
    public long Operand1 { get; set; }
    public long Operand2 { get; set; }
}
