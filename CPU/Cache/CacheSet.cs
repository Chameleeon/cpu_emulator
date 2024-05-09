public class CacheSet
{
    public int LineSize;
    public int Associativity;
    public Dictionary<long, bool> Lines = new Dictionary<long, bool>();

    public CacheSet(int size, int associativity, int lineSize)
    {
        this.LineSize = size;
        this.Associativity = associativity;
    }

    public void AddLine(long tag)
    {
        if (Lines.Count < Associativity)
        {
            // TODO evict
        }
        Lines[tag] = true;
    }

    public bool IsInSet(long tag)
    {
        return Lines.ContainsKey(tag);
    }
}
