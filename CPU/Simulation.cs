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
        Memory.Instance.WriteLong(0, -4886718345);
        Console.WriteLine(Memory.Instance.ReadLong(0));
        byte[] bytes = new byte[] { 0x01, 0x02, 0x03, 0x04 };
        Memory.Instance.WriteBytes(0x100, bytes);
        byte[] res = Memory.Instance.ReadBytes(0x100, 4);
        for (int i = 0; i < 4; i++)
        {
            Console.WriteLine(res[i]);
        }

        Parser.Instance.ParseFile("test.txt");
    }
}
