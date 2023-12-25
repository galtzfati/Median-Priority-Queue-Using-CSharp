using System;
using System.Collections.Generic;

namespace MedianPriorityQueue.Testing
{
    public class Comperator<T> : IComparer<T>
    {
        private Func<T, T, int> m_Comperator;
        public Comperator(Func<T, T, int> i_Comperator)
        {
            m_Comperator = i_Comperator;
        }
        public int Compare(T? x, T? y)
        {
            return m_Comperator(x!, y!);
        }
    }
}
