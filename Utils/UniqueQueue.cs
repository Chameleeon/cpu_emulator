using System;
using System.Collections.Generic;
public class UniqueQueue<T>
{
    private Queue<T> _queue;
    private HashSet<T> _set;

    public UniqueQueue()
    {
        _queue = new Queue<T>();
        _set = new HashSet<T>();
    }

    public void Enqueue(T item)
    {
        if (!_set.Contains(item))
        {
            _queue.Enqueue(item);
            _set.Add(item);
        }
    }

    public T Dequeue()
    {
        T item = _queue.Dequeue();
        _set.Remove(item);
        return item;
    }

    public T Peek()
    {
        return _queue.Peek();
    }

    public int Count
    {
        get { return _queue.Count; }
    }

    public bool IsEmpty
    {
        get { return _queue.Count == 0; }
    }
}
