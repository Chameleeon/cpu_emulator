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

    public void Run()
    {
        Compiler.Instance.Compile("testlru.txt", "lrutest");
        Processor cpu = new Processor();
        cpu.ExecuteProgram("lrutest");
        int[] levelSizes = { 16, 16 };
        CacheSimulator cs = new CacheSimulator(2, 4, levelSizes);
        cs.SimulateLRU("lrutest_mem_access.txt");
        cs.SimulateBelady("lrutest_mem_access.txt");
    }
}
