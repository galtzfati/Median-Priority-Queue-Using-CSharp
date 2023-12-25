using MedianPriorityQueue.PriorityQueue;

namespace MedianPriorityQueue
{
    public interface IMedianPriorityQueue<T> : IPriorityQueue<T>
    {
        T Max { get; }
        T Min { get; }
        T Median { get; }
        void DeleteMax();
        void DeleteMin();
    }
}
