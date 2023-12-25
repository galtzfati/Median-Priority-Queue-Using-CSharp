using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using MedianPriorityQueue.PriorityQueue;

namespace MedianPriorityQueue
{
    public class MedianPriorityQueue<T> : IMedianPriorityQueue<T>
    {
        private class MedianPriorityQueueItem<V> : IIndexable
        {
            public required V Value { get; set; }
            public int Index { get; set; }
            public MedianPriorityQueueItem<V>? Twin { get; set; }
        }

        private Heap<MedianPriorityQueueItem<T>> m_HalfBigMaxHeap; 
        private Heap<MedianPriorityQueueItem<T>> m_HalfBigMinHeap; 
        private Heap<MedianPriorityQueueItem<T>> m_HalfSmallMaxHeap; 
        private Heap<MedianPriorityQueueItem<T>> m_HalfSmallMinHeap;
        private Func<T, T, bool> m_PriorityHandler;

        /// <summary>
        /// Initializes a new instance of the FourHeapMedian class with the specified priority handler.
        /// The priority handler is used to determine the priority order between two items.
        /// It should return true if the first item has a higher priority than the second item of type <typeparamref name="T"/>.
        /// </summary>
        /// <param name="i_PriorityHandler">
        ///   The priority handler delegate that determines the priority order between two items of type <typeparamref name="T"/>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///   Thrown when i_PriorityHandler is null.
        ///   <para>Ensure that the priority handler is provided and is not null.</para>
        /// </exception>
        public MedianPriorityQueue([NotNull] Func<T, T, bool> i_PriorityHandler)
        {
            m_PriorityHandler = i_PriorityHandler ?? throw new ArgumentNullException("i_PriorityHandler cannot be null");
            m_HalfBigMaxHeap = new Heap<MedianPriorityQueueItem<T>>((a, b) => i_PriorityHandler(a.Value, b.Value));
            m_HalfBigMinHeap = new Heap<MedianPriorityQueueItem<T>>((a, b) => !i_PriorityHandler(a.Value, b.Value));
            m_HalfSmallMaxHeap = new Heap<MedianPriorityQueueItem<T>>((a, b) => i_PriorityHandler(a.Value, b.Value));
            m_HalfSmallMinHeap = new Heap<MedianPriorityQueueItem<T>>((a, b) => !i_PriorityHandler(a.Value, b.Value));
        }

        /// <exception cref="InvalidOperationException">Exception is thrown when heap is empty</exception>
        T IPriorityQueue<T>.Top => Max;
        /// <exception cref="InvalidOperationException">Exception is thrown when heap is empty</exception>
        public T Max => m_HalfBigMaxHeap.Count > 0 ? m_HalfBigMaxHeap.Top.Value : m_HalfSmallMaxHeap.Top.Value;
        /// <exception cref="InvalidOperationException">Exception is thrown when heap is empty</exception>
        public T Min => m_HalfSmallMinHeap.Top.Value;
        /// <exception cref="InvalidOperationException">Exception is thrown when heap is empty</exception>
        public T Median => m_HalfSmallMaxHeap.Top.Value;
        public int Count => m_HalfSmallMaxHeap.Count + m_HalfBigMaxHeap.Count;

        public void Insert(T i_Item)
        {
            MedianPriorityQueueItem<T> newItem = new MedianPriorityQueueItem<T>() { Value = i_Item };
            MedianPriorityQueueItem<T> newItemTwin = new MedianPriorityQueueItem<T>() { Value = i_Item };
            newItem.Twin = newItemTwin;
            newItemTwin.Twin = newItem;
            // if newHeapItem has smaller priority than the Median
            if (m_HalfSmallMaxHeap.Count == 0 || m_PriorityHandler.Invoke(m_HalfSmallMaxHeap.Top.Value, newItem.Value))
            {
                m_HalfSmallMaxHeap.Insert(newItem);
                m_HalfSmallMinHeap.Insert(newItemTwin);
                if (m_HalfSmallMaxHeap.Count > m_HalfBigMaxHeap.Count + 1)
                {
                    MedianPriorityQueueItem<T> maxPriorityItem = m_HalfSmallMaxHeap.Top;
                    m_HalfSmallMaxHeap.DeleteTop();
                    m_HalfSmallMinHeap.DeleteAt(maxPriorityItem.Twin!.Index);
                    m_HalfBigMaxHeap.Insert(maxPriorityItem);
                    m_HalfBigMinHeap.Insert(maxPriorityItem.Twin);
                }
            }
            else
            {
                m_HalfBigMaxHeap.Insert(newItem);
                m_HalfBigMinHeap.Insert(newItemTwin);
                if (m_HalfSmallMaxHeap.Count < m_HalfBigMaxHeap.Count)
                {
                    MedianPriorityQueueItem<T> minPriorityItem = m_HalfBigMinHeap.Top;
                    m_HalfBigMinHeap.DeleteTop();
                    m_HalfBigMaxHeap.DeleteAt(minPriorityItem.Twin!.Index);
                    m_HalfSmallMaxHeap.Insert(minPriorityItem);
                    m_HalfSmallMinHeap.Insert(minPriorityItem.Twin);
                }
            }
        }
        public void DeleteMax()
        {
            if (m_HalfBigMaxHeap.Count > 0)
            {
                MedianPriorityQueueItem<T> maxPriorityItem = m_HalfBigMaxHeap.Top;
                m_HalfBigMaxHeap.DeleteTop();
                m_HalfBigMinHeap.DeleteAt(maxPriorityItem.Twin!.Index);

                if (m_HalfSmallMaxHeap.Count > m_HalfBigMaxHeap.Count + 1)
                {
                    MedianPriorityQueueItem<T> halfSmallMaxPriorityItem = m_HalfSmallMaxHeap.Top;
                    m_HalfSmallMaxHeap.DeleteTop();
                    m_HalfSmallMinHeap.DeleteAt(halfSmallMaxPriorityItem.Twin!.Index);
                    m_HalfBigMaxHeap.Insert(halfSmallMaxPriorityItem);
                    m_HalfBigMinHeap.Insert(halfSmallMaxPriorityItem.Twin);
                }
            }
            else // Small heap contains no more than 1 element...
            {
                DeleteMin();
            }
        }
        public void DeleteMin()
        {
            if (m_HalfSmallMaxHeap.Count > 0)
            {
                MedianPriorityQueueItem<T> minPriorityItem = m_HalfSmallMinHeap.Top;
                m_HalfSmallMinHeap.DeleteTop();
                m_HalfSmallMaxHeap.DeleteAt(minPriorityItem.Twin!.Index);

                if (m_HalfSmallMaxHeap.Count < m_HalfBigMaxHeap.Count)
                {
                    MedianPriorityQueueItem<T> halfBigMinPriorityItem = m_HalfBigMinHeap.Top;
                    m_HalfBigMinHeap.DeleteTop();
                    m_HalfBigMaxHeap.DeleteAt(halfBigMinPriorityItem.Twin!.Index);
                    m_HalfSmallMaxHeap.Insert(halfBigMinPriorityItem);
                    m_HalfSmallMinHeap.Insert(halfBigMinPriorityItem.Twin);
                }
            }
        }
        void IPriorityQueue<T>.DeleteTop()
        {
            DeleteMax();
        }
        public void Clear()
        {
            m_HalfBigMaxHeap.Clear();
            m_HalfBigMinHeap.Clear();
            m_HalfSmallMaxHeap.Clear();
            m_HalfSmallMinHeap.Clear();
        }
        public IEnumerator<T> GetEnumerator()
        {
            foreach (MedianPriorityQueueItem<T> item in m_HalfSmallMaxHeap)
            {
                yield return item.Value;
            }
            foreach (MedianPriorityQueueItem<T> item in m_HalfBigMaxHeap)
            {
                yield return item.Value;
            }
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
