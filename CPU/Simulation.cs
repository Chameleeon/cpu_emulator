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
        MemoryManager mm = MemoryManager.Initialize(2048, 4096);
        long address = mm.TranslateAddress(0x101);
        Memory.Instance.WriteLong(0, -0x1234556789);
        Console.WriteLine(Convert.ToString(-0x123456789, 2));
        Console.WriteLine(Memory.Instance.ReadLong(0));
    }
}
