public class Processor : IProcessor
{
    public IRegister[] Registers { get; set; } = new Register64[4];

    public IMMU MemoryManager { get; set; }

    public int ProgramCounter { get; set; }

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
