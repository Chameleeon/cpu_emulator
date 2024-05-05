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
        Memory mem = new Memory(4096);
        mem.WriteLong(0, -4886718345);
        Console.WriteLine(mem.ReadLong(0));
        byte[] bytes = new byte[] { 0x01, 0x02, 0x03, 0x04, 0x10 };
        mem.WriteBytes(0x100, bytes);
        byte[] res = mem.ReadBytes(0x100, 5);

        for (int i = 0; i < 5; i++)
        {
            Console.WriteLine(res[i]);
        }

        new Parser();
        // Parser.ParseInstructions("Instrukcije.txt");

    }
}
