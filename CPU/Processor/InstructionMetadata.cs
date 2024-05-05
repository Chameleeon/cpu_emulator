
public class InstructionMetadata
{
    public string Mnemonic { get; set; }
    public byte Opcode { get; set; }

    public InstructionMetadata(string mnemonic, byte opcode)
    {
        Mnemonic = mnemonic;
        Opcode = opcode;
    }
}


