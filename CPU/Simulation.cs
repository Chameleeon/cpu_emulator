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
        Console.WriteLine(address);
    }
}
