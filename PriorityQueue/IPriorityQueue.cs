using System.Collections.Generic;

namespace MedianPriorityQueue.PriorityQueue
{
    public interface IPriorityQueue<T> : IEnumerable<T>
    {
        int Count { get; }
        T Top { get; }
        void Clear();
        void Insert(T i_Item);
        void DeleteTop();
    }
}
