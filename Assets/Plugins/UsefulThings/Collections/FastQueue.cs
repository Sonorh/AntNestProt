using UnityEngine;
using System.Collections;

namespace PM.UsefulThings
{
	public class FastQueue<T> where T : class
	{
		public struct Enumerator
		{
			private int m_index;
			private readonly FastQueue<T> m_queue;

			public Enumerator(FastQueue<T> queue)
			{
				m_queue = queue;
				m_index = -1;
			}

			public void Reset()
			{
				m_index = -1;
			}

			public T Current
			{
				get { return m_queue[m_index]; }
			}

			public bool MoveNext()
			{
				if (m_index + 1 == m_queue.count)
					return false;

				++m_index;
				return true;
			}
		}

		private const int minCapacity = 4;
		private int m_head = 0;
		private int m_capacity = minCapacity;
		private int m_count;
		private T[] m_data;

		public int count
		{
			get { return m_count; }
		}

		public int capacity
		{
			get { return m_capacity; }
			set
			{
				int newCapacity = Mathf.Max(value, Mathf.Max(m_count, minCapacity));
				T[] temp = new T[newCapacity];
				int length = Mathf.Min(m_capacity - m_head, m_count);
				System.Array.Copy(m_data, m_head, temp, 0, length);
				System.Array.Copy(m_data, 0, temp, length, m_count - length);
				m_data = temp;
				m_head = 0;
				m_capacity = newCapacity;
			}
		}

		public FastQueue()
		{
			m_data = new T[m_capacity];
		}

		public FastQueue(int capacity)
		{
			m_capacity = capacity;
			m_data = new T[m_capacity];
		}

		public void PushTail(T element)
		{
			if (m_count + 1 > m_capacity)
				capacity *= 2;
			++m_count;
			m_data[tail] = element;
		}

		public T PopTail()
		{
			T element = m_data[tail];
			m_data[tail] = null;
			--m_count;
			return element;
		}

		public void PushHead(T element)
		{
			++m_count;
			if (m_count > m_capacity)
				capacity *= 2;
			if (m_count > 1)
				m_head = Decrement(m_head);
			m_data[m_head] = element;
		}

		public T PopHead()
		{
			--m_count;
			T element = m_data[m_head];
			m_data[m_head] = null;
			m_head = Increment(m_head);
			return element;
		}

		public T PeekHead()
		{
			return m_data[m_head];
		}

		public T PeekTail()
		{
			return m_data[tail];
		}

		public T this[int index]
		{
			get { return m_data[(m_head + index) % m_capacity]; }
			set { m_data[(m_head + index) % m_capacity] = value; }
		}

		public void Clear()
		{
			m_head = 0;
			m_count = 0;
		}

		public Enumerator GetEnumerator()
		{
			return new Enumerator(this);
		}

		private int tail
		{
			get { return (m_head + m_count - 1) % m_capacity; }
		}

		private int Increment(int index)
		{
			return (index + 1) % m_capacity;
		}

		private int Decrement(int index)
		{
			return (index + m_capacity - 1) % m_capacity;
		}
	}
}