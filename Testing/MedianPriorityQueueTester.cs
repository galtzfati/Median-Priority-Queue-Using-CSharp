using System;
using System.Collections.Generic;

namespace MedianPriorityQueue.Testing
{
    public class MedianPriorityQueueTester<T> : IMedianPriorityQueueTester<T>
    {
        private IMedianPriorityQueue<T> m_Mpq1;
        private IMedianPriorityQueue<T> m_Mpq2;
        private IComparer<T> m_Comparer;
        public MedianPriorityQueueTester(IMedianPriorityQueue<T> i_Mpq1, IMedianPriorityQueue<T> i_Mpq2, IComparer<T> i_Comparer)
        {
            m_Mpq1 = i_Mpq1;
            m_Mpq2 = i_Mpq2;
            m_Comparer = i_Comparer;
        }

        private bool runTest()
        {
            bool bothAreEmpty = m_Mpq1.Count == m_Mpq2.Count && m_Mpq1.Count == 0;
            return bothAreEmpty || m_Mpq1.Count == m_Mpq2.Count
                            && m_Comparer.Compare(m_Mpq1.Max, m_Mpq2.Max) == 0
                            && m_Comparer.Compare(m_Mpq1.Min, m_Mpq2.Min) == 0
                            && m_Comparer.Compare(m_Mpq1.Median, m_Mpq2.Median) == 0;
        }
        public bool TestInsert(T i_Item)
        {
            m_Mpq1.Insert(i_Item);
            m_Mpq2.Insert(i_Item);
            return runTest();
        }
        public bool TestDeleteMax()
        {
            m_Mpq1.DeleteMax();
            m_Mpq2.DeleteMax();
            return runTest();
        }
        public bool TestDeleteMin()
        {
            m_Mpq1.DeleteMin();
            m_Mpq2.DeleteMin();
            return runTest();
        }
        public bool TestClear()
        {
            m_Mpq1.Clear();
            m_Mpq2.Clear();
            return runTest();
        }
    }
}
