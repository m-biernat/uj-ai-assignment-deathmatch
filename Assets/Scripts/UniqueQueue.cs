using System.Collections.Generic;

public class UniqueQueue<T>
{
    private readonly Queue<T> _queue = new Queue<T>();
    private HashSet<T> _alreadyAdded = new HashSet<T>();

    public virtual void Enqueue(T item)
    {
        if (_alreadyAdded.Add(item)) { _queue.Enqueue(item); }
    }
    public int Count { get { return _queue.Count; } }

    public virtual T Dequeue()
    {
        T item = _queue.Dequeue();
        _alreadyAdded.Remove(item);
        return item;
    }
}
