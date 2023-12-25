using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MedianPriorityQueue.Testing
{
    public class ArrayMedian<T> : IMedianPriorityQueue<T>
    {
        private List<T> m_Items = new List<T>();
        private Func<T, T, bool> m_PriorityHandler;

        /// <param name="i_PriorityHandler">
        ///   The priority handler delegate that determines the priority order between two items of type <typeparamref name="T"/>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///   Thrown when i_PriorityHandler is null.
        ///   <para>Ensure that the priority handler is provided and is not null.</para>
        /// </exception>
        public ArrayMedian(Func<T, T, bool> i_PriorityHandler)
        {
            m_PriorityHandler = i_PriorityHandler ?? throw new ArgumentNullException("i_PriorityHandler cannot be null");
        }

        public T Max => m_Items.OrderByDescending(item => item, Comparer<T>.Create((a, b) => m_PriorityHandler(a, b) ? 1 : m_PriorityHandler(b, a) ? -1 : 0)).FirstOrDefault() ?? throw new InvalidOperationException("Array is empty");
        public T Min => m_Items.OrderBy(item => item, Comparer<T>.Create((a, b) => m_PriorityHandler(a, b) ? 1 : m_PriorityHandler(b, a) ? -1 : 0)).FirstOrDefault() ?? throw new InvalidOperationException("Array is empty");
        public T Median => m_Items.Count > 0 ? m_Items.OrderBy(item => item, Comparer<T>.Create((a, b) => m_PriorityHandler(a, b) ? 1 : m_PriorityHandler(b, a) ? -1 : 0)).ToList()[m_Items.Count % 2 == 0 ? (m_Items.Count - 1) / 2 : m_Items.Count / 2] : throw new InvalidOperationException("Array is empty");
        public int Count => m_Items.Count;
        public T Top => Max;

        public void Clear()
        {
            m_Items.Clear();
        }
        public void DeleteMax()
        {
            m_Items.Remove(Max);
        }
        public void DeleteMin()
        {
            m_Items.Remove(Min);
        }
        public void DeleteTop()
        {
            DeleteMax();
        }
        public void Insert(T i_Item)
        {
            m_Items.Add(i_Item);
        }
        public IEnumerator<T> GetEnumerator()
        {
            return m_Items.GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}