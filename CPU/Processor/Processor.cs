public class Processor : IProcessor
{
    public IRegister[] _registers { get; set; }

    public int _programCounter { get; set; }

    public IMemSpace _memory { get; set; }

    public Processor(int pageSize = 4096, int registerCount = 4)
    {
        _memory = new Memory(pageSize);
        _programCounter = 0;
        _registers = new Register64[registerCount];
        for (int i = 0; i < _registers.Length; i++)
        {
            _registers[i] = new Register64();
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
