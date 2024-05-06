public class Simulation
{
    private static Simulation _instance;
    public static Simulation Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new Simulation();
            }
            return _instance;
        }
    }
    private Simulation()
    { }

    public void run()
    {
        Compiler.Instance.Compile("testt.txt", "program2");
        Processor cpu = new Processor();
        cpu.ExecuteProgram("program2");
        byte[] instructions = cpu._memory.ReadBytes(0, 60);
        // foreach (byte b in instructions)
        // {
        //     Console.Write(b.ToString("X2") + " ");
        // }
    }
}
