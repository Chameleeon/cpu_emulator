namespace UnitTests;

public class CacheUnitTest
{

    [Fact]
    public void CacheTest()
    {
        Compiler.Instance.Compile("test_programs/cache_test.txt", "test_programs/cache_test");
        Processor cpu = new Processor();
        cpu.ExecuteProgram("test_programs/cache_test");
        int[] levelSizes = { 16, 16 };
        int[] associativity = { 2, 2 };
        CacheSimulator cs = new CacheSimulator(2, levelSizes, associativity, 4);
        Tuple<int, int> lruHM = cs.SimulateLRU("test_programs/cache_test_mem_access.txt");
        Tuple<int, int> beladyHM = cs.SimulateBelady("test_programs/cache_test_mem_access.txt");
        Assert.Equal(15, lruHM.Item1);
        Assert.Equal(24, lruHM.Item2);
        Assert.Equal(23, beladyHM.Item1);
        Assert.Equal(16, beladyHM.Item2);
    }
}
