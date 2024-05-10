using System.Collections.Generic;
using System.Linq;
public class CacheLevel
{
    private int _numOfLines;
    private int _lineSize;
    private int _offsetBits;
    private int _indexBits;
    private int _tagBits;
    private int _addressSpaceSize;            // in bits
    private int _numOfSets;
    private int _associativity;
    private Dictionary<long, Queue<long>>? _lruSets;
    private Dictionary<long, Dictionary<long, int>>? _beladyNextOccurence;
    private Dictionary<long, int>? _beladyNextOccurenceGlobal;
    public Dictionary<long, HashSet<long>>? _beladySets;
    public CacheLevel(int levelSize, int lineSize, int associativity)
    {

        this._numOfLines = levelSize / lineSize;
        this._lineSize = lineSize;
        this._numOfSets = _numOfLines / associativity;
        this._offsetBits = (int)Math.Log(lineSize, 2);
        this._indexBits = (int)Math.Log(_numOfSets, 2);
        this._tagBits = _addressSpaceSize - _indexBits - _offsetBits;
        this._associativity = associativity;
    }

    public bool IsInLRUCache(long address)
    {
        if (_lruSets == null)
        {
            _lruSets = new Dictionary<long, Queue<long>>();
        }
        int index = (int)((long)address >> _offsetBits) & (1 << _indexBits) - 1;
        int tag = (int)((long)address >> (_offsetBits + _indexBits)) & (1 << _tagBits) - 1;
        if (_lruSets.ContainsKey(index))
        {
            return _lruSets[index].Contains(tag);
        }

        return false;
    }

    public void LRUAdd(long address)
    {
        if (_lruSets == null)
        {
            _lruSets = new Dictionary<long, Queue<long>>();
        }
        int index = (int)((long)address >> _offsetBits) & (1 << _indexBits) - 1;
        int tag = (int)((long)address >> (_offsetBits + _indexBits)) & (1 << _tagBits) - 1;

        if (!_lruSets.ContainsKey(index))
        {
            _lruSets[index] = new Queue<long>();
        }
        if (_lruSets[index].Contains(tag))
        {
            List<long> tmp = _lruSets[index].ToList();
            tmp.Remove(tag);
            _lruSets[index] = new Queue<long>(tmp);
        }
        _lruSets[index].Enqueue(tag);
    }

    public long Evict(long address)
    {
        int index = (int)((long)address >> _offsetBits) & (1 << _indexBits) - 1;
        int tag = (int)((long)address >> (_offsetBits + _indexBits)) & (1 << _tagBits) - 1;
        long evictedAddressBlock = -1;
        if (_lruSets.ContainsKey(index) && _lruSets[index].Count >= _numOfLines)
        {
            long tagToEvict = _lruSets[index].Dequeue();
            evictedAddressBlock = tagToEvict << (_offsetBits + _indexBits) | index << _offsetBits;
        }
        return evictedAddressBlock;
    }

    public bool IsInBeladyCache(long address)
    {
        if (_beladySets == null)
        {
            _beladySets = new Dictionary<long, HashSet<long>>();
        }
        int index = (int)((long)address >> _offsetBits) & (1 << _indexBits) - 1;
        int tag = (int)((long)address >> (_offsetBits + _indexBits)) & (1 << _tagBits) - 1;
        if (_beladySets.ContainsKey(index))
        {
            return _beladySets[index].Contains(tag);
        }
        return false;
    }

    public void BeladyAdd(long address)
    {
        if (_beladySets == null)
        {
            _beladySets = new Dictionary<long, HashSet<long>>();
        }
        int index = (int)((long)address >> _offsetBits) & (1 << _indexBits) - 1;
        int tag = (int)((long)address >> (_offsetBits + _indexBits)) & (1 << _tagBits) - 1;

        if (!_beladySets.ContainsKey(index))
        {
            _beladySets[index] = new HashSet<long>();
        }
        _beladySets[index].Add(tag);
    }

    public long EvictBelady(long address)
    {

        int index = (int)((long)address >> _offsetBits) & (1 << _indexBits) - 1;
        int tag = (int)((long)address >> (_offsetBits + _indexBits)) & (1 << _tagBits) - 1;
        long evictedAddressBlock = -1;
        if (_beladySets.ContainsKey(index) && _beladySets[index].Count >= _numOfLines)
        {
            long tagToEvict = GetTagToEvict(index);

            evictedAddressBlock = tagToEvict << (_offsetBits + _indexBits) | index << _offsetBits;
            _beladySets[index].Remove(tagToEvict);
        }
        return evictedAddressBlock;
    }

    public void InitializeBeladyOccurence(string path, Dictionary<long, int> nextOccurence)
    {
        _beladyNextOccurenceGlobal = nextOccurence;
        if (_beladyNextOccurence == null)
        {
            _beladyNextOccurence = new Dictionary<long, Dictionary<long, int>>();

        }

        using (StreamReader reader = new StreamReader(path))
        {
            string? line;
            while ((line = reader.ReadLine()) != null)
            {
                long address = long.Parse(line, System.Globalization.NumberStyles.HexNumber);
                int index = (int)((long)address >> _offsetBits) & (1 << _indexBits) - 1;
                int tag = (int)((long)address >> (_offsetBits + _indexBits)) & (1 << _tagBits) - 1;
                if (!_beladyNextOccurence.ContainsKey(index))
                {
                    _beladyNextOccurence[index] = new Dictionary<long, int>();
                }
                _beladyNextOccurence[index][tag] = int.MaxValue;
            }
        }
    }

    public long GetTagToEvict(int index)
    {
        long maxKey = -1;
        int maxValue = int.MinValue;

        foreach (int value in _beladySets[index])
        {
            if (_beladyNextOccurence[index].TryGetValue(value, out int nextOccurence) && nextOccurence > maxValue)
            {
                maxValue = nextOccurence;
                maxKey = value;
            }
        }
        return maxKey;
    }

    public void UpdateBeladyOccurence(long address, int nextOccurence)
    {
        int index = (int)((long)address >> _offsetBits) & (1 << _indexBits) - 1;
        int tag = (int)((long)address >> (_offsetBits + _indexBits)) & (1 << _tagBits) - 1;
        if (nextOccurence < 0)
            nextOccurence = int.MaxValue;
        _beladyNextOccurence[index][tag] = nextOccurence;
    }
}
