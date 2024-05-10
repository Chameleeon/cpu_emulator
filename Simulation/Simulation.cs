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

    public static void Main(string[] args)
    {
        {
            if (args.Length == 0)
            {
                Console.WriteLine("No arguments provided. Use -h for help.");
                Environment.Exit(0);
            }

            if (args[0] == "-h")
            {
                Console.WriteLine("Usage: Simulation.exe " +
                        "\n-in [input_file] \n-o [output_file] \n-r [number_of_registers] : default 4" +
                        "\n-p [page_size] : default 4096 \n-l [number_of_cache_levels] : default 2" +
                        "\n-a [level1_associativity,level2_associativity...] : default 2" +
                        "\n-c [cache_line_size] : default 4 \n-cl [cache_level1_size,cache_level2_size...] in bytes : default 16");
                Environment.Exit(0);
            }

            if (args.Length < 2)
            {
                Console.WriteLine("Not enough arguments provided. Use -h for help.");
                Environment.Exit(0);
            }

            string inputFile = "";
            string outputFile = "";
            int registers = 4;
            int pageSize = 4096;
            int cacheLevels = 2;
            int[] associativity = { 2, 2 };
            int cacheLineSize = 4;
            int[] levelSizes = { 16, 16 };

            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == "-in")
                {
                    inputFile = args[i + 1];
                    if (!File.Exists(inputFile))
                    {
                        Console.WriteLine("Input file does not exist.");
                        Environment.Exit(0);
                    }
                    i++;
                }

                if (args[i] == "-o")
                {
                    outputFile = args[i + 1];
                    i++;
                }

                if (args[i] == "-r")
                {
                    registers = int.Parse(args[i + 1]);
                    i++;
                }

                if (args[i] == "-p")
                {
                    pageSize = int.Parse(args[i + 1]);
                    i++;
                }

                if (args[i] == "-l")
                {
                    cacheLevels = int.Parse(args[i + 1]);
                    i++;
                }

                if (args[i] == "-a")
                {
                    associativity = new int[cacheLevels];
                    for (int j = 0; j < args[i + 1].Split(',').Length; j++)
                    {
                        associativity[j] = int.Parse(args[i + 1].Split(',')[j]);
                    }
                    i++;
                }

                if (args[i] == "-c")
                {
                    cacheLineSize = int.Parse(args[i + 1]);
                    i++;
                }

                if (args[i] == "-cl")
                {
                    levelSizes = new int[cacheLevels];
                    for (int j = 0; j < args[i + 1].Split(',').Length; j++)
                    {
                        levelSizes[j] = int.Parse(args[i + 1].Split(',')[j]);
                    }
                    i++;
                }

            }
            Compiler.Instance.Compile(inputFile, outputFile);
            Processor cpu = new Processor(pageSize, registers);
            cpu.ExecuteProgram(outputFile);
            CacheSimulator cs = new CacheSimulator(cacheLevels, levelSizes, associativity, cacheLineSize);
            cs.SimulateLRU(outputFile + "_mem_access.txt");
            cs.SimulateBelady(outputFile + "_mem_access.txt");

        }
    }
}
