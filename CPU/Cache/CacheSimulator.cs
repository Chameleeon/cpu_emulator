using System.Text.RegularExpressions;
public class CacheSimulator
{
    private int _numOfLevels;
    private int[] _levelSizes;
    private CacheLevel[] _cacheLevels;
    private int[] _associativityPerLevel;
    private int _cacheLineSize;

    public CacheSimulator(int numOfLevels, int[] levelSizes, int[] associativityPerLevel, int cacheLineSize)
    {
        this._numOfLevels = numOfLevels;
        this._cacheLevels = new CacheLevel[numOfLevels];
        this._levelSizes = levelSizes;
        this._associativityPerLevel = associativityPerLevel;
        this._cacheLineSize = cacheLineSize;
    }

    public Tuple<int, int> SimulateLRU(string inputFile)
    {
        for (int i = 0; i < _numOfLevels; i++)
        {
            _cacheLevels[i] = new CacheLevel(_levelSizes[i], _cacheLineSize, _associativityPerLevel[i]);
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

                long evictedAddress = -1;
                long evictedAddress2 = -1;
                for (int i = 0; i < _numOfLevels; i++)
                {
                    if (_cacheLevels[i].IsInLRUCache(address))
                    {
                        hit = true;
                        hits++;
                        if (i > 0)
                        {
                            evictedAddress = _cacheLevels[0].Evict(address);
                        }
                        _cacheLevels[0].LRUAdd(address);
                        break;
                    }
                }

                if (evictedAddress != -1)
                {
                    for (int i = 1; i < _numOfLevels; i++)
                    {
                        if (evictedAddress != -1)
                        {
                            evictedAddress = _cacheLevels[i].Evict(evictedAddress2);
                            _cacheLevels[i].LRUAdd(evictedAddress);
                        }
                    }
                }

                if (!hit)
                {
                    misses++;
                    evictedAddress2 = _cacheLevels[0].Evict(address);
                    for (int i = 1; i < _numOfLevels; i++)
                    {
                        if (evictedAddress2 != -1)
                        {
                            evictedAddress = _cacheLevels[i].Evict(evictedAddress2);
                            _cacheLevels[i].LRUAdd(evictedAddress2);
                        }
                    }
                    _cacheLevels[0].LRUAdd(address);
                }
            }
        }
        Console.WriteLine("LRU>>: hits: " + hits + " misses: " + misses);
        return new Tuple<int, int>(hits, misses);
    }

    public Tuple<int, int> SimulateBelady(string inputFile)
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
                long evictedAddress = -1;
                bool hit = false;
                long address = memoryAccesses[i];
                for (int j = 0; j < _numOfLevels; j++)
                {
                    if (_cacheLevels[j].IsInBeladyCache(address))
                    {
                        hit = true;
                        hits++;
                        if (j > 0)
                        {
                            evictedAddress = _cacheLevels[0].Evict(address);
                        }
                        _cacheLevels[0].BeladyAdd(address);
                        break;
                    }
                }


                if (evictedAddress != -1)
                {
                    for (int j = 1; j < _numOfLevels; j++)
                    {
                        if (evictedAddress != -1)
                        {
                            _cacheLevels[j].LRUAdd(evictedAddress);
                            evictedAddress = _cacheLevels[j].Evict(evictedAddress);
                        }
                    }
                }

                if (!hit)
                {
                    misses++;
                    long evictedAddress2 = _cacheLevels[0].EvictBelady(address);
                    for (int j = 1; j < _numOfLevels; j++)
                    {
                        if (evictedAddress2 != -1)
                        {
                            evictedAddress = _cacheLevels[j].EvictBelady(evictedAddress2);
                            _cacheLevels[j].BeladyAdd(evictedAddress2);
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
            return new Tuple<int, int>(hits, misses);
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
