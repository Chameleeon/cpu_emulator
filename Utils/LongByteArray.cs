namespace Utils
{
    public class LongByteArray
    {
        private Dictionary<long, byte> _memory = new Dictionary<long, byte>();
        private long _maxSize;

        public byte this[long index]
        {
            get
            {
                if (_memory.ContainsKey(index))
                    return _memory[index];
                else
                    throw new IndexOutOfRangeException();
            }
            set
            {
                if (index < _maxSize)
                {
                    _memory[index] = value;
                }
                else
                {
                    throw new IndexOutOfRangeException();
                }
            }
        }

        public LongByteArray(long maxSize)
        {
            _maxSize = maxSize;
        }
    }
}
