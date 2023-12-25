namespace MedianPriorityQueue.Testing
{
    public interface IMedianPriorityQueueTester<T>
    {
        public bool TestInsert(T i_Item);
        public bool TestDeleteMax();
        public bool TestDeleteMin();
        public bool TestClear();
    }
}
