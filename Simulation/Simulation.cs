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
        Compiler.Instance.Compile("cache_test.txt", "cache_test");
        Processor cpu = new Processor();
        cpu.ExecuteProgram("cache_test");
        int[] levelSizes = { 16, 16 };
        int[] associativity = { 2, 2 };
        CacheSimulator cs = new CacheSimulator(2, levelSizes, associativity, 4);
        cs.SimulateLRU("cache_test_mem_access.txt");
        cs.SimulateBelady("cache_test_mem_access.txt");
    }
}
