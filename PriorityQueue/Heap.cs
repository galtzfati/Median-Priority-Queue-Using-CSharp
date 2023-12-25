using System;
using System.Collections;
using System.Collections.Generic;

namespace MedianPriorityQueue.PriorityQueue
{
    public class Heap<T> : IPriorityQueue<T>
    {
        private class HeapItem<V>
        {
            public required V Value { get; set; }
            private int m_Index;
            public int Index
            {
                get => m_Index;
                set
                {
                    m_Index = value;
                    if (Value is IIndexable indexable)
                    {
                        indexable.Index = m_Index;
                    }
                }
            }
            public int ParentIndex => (Index - 1) / 2;
            public int LeftChildIndex => Index * 2 + 1;
            public int RightChildIndex => Index * 2 + 2;
        }
        private class HeapEnumerator<V> : IEnumerator<V>
        {
            private IEnumerator<HeapItem<V>?> m_Enumerator;
            private int m_Limit;
            private int m_Count = 0;

            public HeapEnumerator(IEnumerable<HeapItem<V>?> i_Items, int i_Limit)
            {
                m_Enumerator = i_Items.GetEnumerator();
                m_Limit = i_Limit;
            }
            public bool MoveNext()
            {
                m_Count++;
                return m_Enumerator.MoveNext() && m_Count <= m_Limit;
            }
            public V Current => m_Enumerator.Current!.Value!;
            object IEnumerator.Current => Current!;
            public void Reset()
            {
                m_Count = 0;
                m_Enumerator.Reset();
            }
            public void Dispose()
            {
                m_Enumerator.Dispose();
            }
        }

        private List<HeapItem<T>?> m_Items = new List<HeapItem<T>?>();
        private Func<T, T, bool> m_PriorityHandler;
        private int m_NumOfEmptySpaces = 0;

        public Heap(Func<T, T, bool> i_PriorityHandler)
        {
            m_PriorityHandler = i_PriorityHandler;
        }

        /// <exception cref="InvalidOperationException">Exception is thrown when heap is empty</exception>
        public T Top => Count > 0 ? m_Items[0]!.Value : throw new InvalidOperationException("Heap is Empty");
        public int Count => m_Items.Count - m_NumOfEmptySpaces;
        public void Insert(T i_Item)
        {
            HeapItem<T> newHeapItem = new HeapItem<T> { Value = i_Item };
            if (m_NumOfEmptySpaces > 0)
            {
                m_Items[Count] = newHeapItem;
                newHeapItem.Index = Count;
                m_NumOfEmptySpaces--;
            }
            else
            {
                m_Items.Add(newHeapItem);
                newHeapItem.Index = Count - 1;
            }
            fixHeapUpward(Count - 1);
        }
        public void DeleteTop()
        {
            if (Count > 0)
            {
                m_NumOfEmptySpaces++;
                swapItems(0, Count);
                m_Items[Count] = null;
                fixHeapDownward(0);

                if (m_Items.Count <= 2 * m_NumOfEmptySpaces)
                {
                    m_Items.RemoveRange(m_Items.Count - m_NumOfEmptySpaces, m_NumOfEmptySpaces);
                    m_NumOfEmptySpaces = 0;
                }
            }
        }
        public void Clear()
        {
            m_Items.Clear();
            m_NumOfEmptySpaces = 0;
        }
        internal void DeleteAt(int i_Index)
        {
            if (Count > 0 && i_Index >= 0 && i_Index < Count)
            {
                swapItems(i_Index, Count - 1);
                m_Items[Count - 1] = null;
                m_NumOfEmptySpaces++;

                fixHeapUpward(i_Index);
                fixHeapDownward(i_Index);

                if (m_Items.Count <= 2 * m_NumOfEmptySpaces)
                {
                    m_Items.RemoveRange(m_Items.Count - m_NumOfEmptySpaces, m_NumOfEmptySpaces);
                    m_NumOfEmptySpaces = 0;
                }
            }
        }
        private void fixHeapUpward(int i_CurrentItemIndex)
        {
            while (i_CurrentItemIndex > 0 && m_Items[i_CurrentItemIndex] != null && m_PriorityHandler.Invoke(m_Items[i_CurrentItemIndex]!.Value, m_Items[m_Items[i_CurrentItemIndex]!.ParentIndex]!.Value))
            {
                swapItems(i_CurrentItemIndex, m_Items[i_CurrentItemIndex]!.ParentIndex);
                i_CurrentItemIndex = m_Items[i_CurrentItemIndex]!.ParentIndex;
            }
        }
        private void fixHeapDownward(int i_CurrentItemIndex)
        {
            if (i_CurrentItemIndex >= 0 && i_CurrentItemIndex < Count)
            {
                int leftChildIndex = m_Items[i_CurrentItemIndex]?.LeftChildIndex ?? -1;
                int rightChildIndex = m_Items[i_CurrentItemIndex]?.RightChildIndex ?? -1;

                int higherPriorityIndex = leftChildIndex >= 0 && leftChildIndex < Count && m_PriorityHandler.Invoke(m_Items[leftChildIndex]!.Value, m_Items[i_CurrentItemIndex]!.Value) ? leftChildIndex : i_CurrentItemIndex;
                higherPriorityIndex = rightChildIndex >= 0 && rightChildIndex < Count && m_PriorityHandler.Invoke(m_Items[rightChildIndex]!.Value, m_Items[higherPriorityIndex]!.Value) ? rightChildIndex : higherPriorityIndex;

                if (higherPriorityIndex != i_CurrentItemIndex)
                {
                    swapItems(higherPriorityIndex, i_CurrentItemIndex);
                    fixHeapDownward(higherPriorityIndex);
                }
            }
        }
        private void swapItems(int index1, int index2)
        {
            HeapItem<T>? tempItem = m_Items[index1];
            m_Items[index1] = m_Items[index2];
            m_Items[index2] = tempItem;

            m_Items[index1]!.Index = index1;
            m_Items[index2]!.Index = index2;
        }
        public IEnumerator<T> GetEnumerator()
        {
            return new HeapEnumerator<T>(m_Items, Count);
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}