public class Processor : IProcessor
{
    public IRegister[] Registers { get; set; } = new Register64[4];

    public IMMU MemManager { get; set; }

    public int ProgramCounter { get; set; }

    public Processor(int memorySizeInPages = 2048, int pageSize = 4096)
    {
        MemManager = MemoryManager.Initialize(memorySizeInPages, pageSize);
        ProgramCounter = 0;
        for (int i = 0; i < Registers.Length; i++)
        {
            Registers[i] = new Register64();
        }
    }

    public void ExecuteInstruction()
    {

    }

    public void ParseInstructions(string filePath)
    {
        try
        {
            string[] lines = File.ReadAllLines(filePath);
            foreach (string line in lines)
            {

            }
        }
        catch (Exception e)
        {
            Console.WriteLine("An error occurred: " + e.Message);
        }
    }
}
