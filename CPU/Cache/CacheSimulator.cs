using System.Text.RegularExpressions;
public class CacheSimulator
{
    private int _numOfLevels;
    private int _cacheLineSize;
    private int[] _levelSizes;
    private CacheLevel[] _cacheLevels;

    public CacheSimulator(int numOfLevels, int cacheLineSize, int[] levelSizes)
    {
        this._numOfLevels = numOfLevels;
        this._cacheLineSize = cacheLineSize;
        this._cacheLevels = new CacheLevel[numOfLevels];
        this._levelSizes = levelSizes;
    }

    public void SimulateLRU(string inputFile)
    {
        for (int i = 0; i < _numOfLevels; i++)
        {
            _cacheLevels[i] = new CacheLevel(_levelSizes[i], 4, 2);
        }
        int hits = 0;
        int misses = 0;
        using (StreamReader reader = new StreamReader(inputFile))
        {
            string? line;
            while ((line = reader.ReadLine()) != null)
            {
                bool hit = false;
                long address = long.Parse(line, System.Globalization.NumberStyles.HexNumber);

                for (int i = 0; i < _numOfLevels; i++)
                {
                    if (_cacheLevels[i].IsInLRUCache(address))
                    {
                        hit = true;
                        hits++;
                        _cacheLevels[i].LRUAdd(address);
                        break;
                    }
                }

                if (!hit)
                {
                    misses++;
                    long evictedAddress = _cacheLevels[0].Evict(address);
                    for (int i = 1; i < _numOfLevels; i++)
                    {
                        if (evictedAddress != -1)
                        {
                            _cacheLevels[i].LRUAdd(evictedAddress);
                            evictedAddress = _cacheLevels[i].Evict(evictedAddress);
                        }
                    }
                    _cacheLevels[0].LRUAdd(address);
                }
            }
        }
        Console.WriteLine("LRU>>: hits: " + hits + " misses: " + misses);
    }

    public void SimulateBelady(string inputFile)
    {
        int hits = 0;
        int misses = 0;
        Dictionary<long, int> nextOccurence = new Dictionary<long, int>();

        for (int i = 0; i < _numOfLevels; i++)
        {
            _cacheLevels[i].InitializeBeladyOccurence(inputFile, nextOccurence);
        }

        using (StreamReader reader = new StreamReader(inputFile))
        {
            string? line;
            List<long> memoryAccesses = new List<long>();
            while ((line = reader.ReadLine()) != null)
            {
                long address = long.Parse(line, System.Globalization.NumberStyles.HexNumber);
                nextOccurence[address] = int.MaxValue;
                memoryAccesses.Add(address);
            }

            for (int i = 0; i < memoryAccesses.Count; i++)
            {
                bool hit = false;
                long address = memoryAccesses[i];
                for (int j = 0; j < _numOfLevels; j++)
                {
                    if (_cacheLevels[j].IsInBeladyCache(address))
                    {
                        hit = true;
                        hits++;
                        _cacheLevels[j].BeladyAdd(address);
                        break;
                    }
                }

                if (!hit)
                {
                    misses++;
                    long evictedAddress = _cacheLevels[0].EvictBelady(address);
                    for (int j = 1; j < _numOfLevels; j++)
                    {
                        if (evictedAddress != -1)
                        {
                            _cacheLevels[j].BeladyAdd(evictedAddress);
                            evictedAddress = _cacheLevels[j].EvictBelady(evictedAddress);
                        }
                    }
                    _cacheLevels[0].BeladyAdd(address);
                }
                if (i != memoryAccesses.Count - 1)
                {
                    UpdateBeladyOccurence(address, memoryAccesses.IndexOf(address, i + 1));
                }
            }
            Console.WriteLine("BELADY>>: hits: " + hits + " misses: " + misses);
        }
    }

    public void UpdateBeladyOccurence(long address, int nextOccurence)
    {
        for (int i = 0; i < _numOfLevels; i++)
        {
            _cacheLevels[i].UpdateBeladyOccurence(address, nextOccurence);
        }
    }
}
