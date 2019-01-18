using PM.UsefulThings.Extensions;
using System;
using System.Collections.Generic;

using UIBinding.Base;

namespace UIBinding
{
	public class ListProperty : Property<IEnumerable<BaseListElementData>>
	{
		public event Action<BaseListElementData> OnElementAdded;
		public event Action<int, BaseListElementData> OnElementInserted;
		public event Action<int> OnElementRemoved;
		public event Action<int> OnScrollToElement;
		public event Action<int, BaseListElementData> OnElementChanged;
		public event Action<int, int> OnElementMoved;
		public event Action OnFullReorder;
		public event Action<BaseListElementData> OnClick;
		public event Action<BaseListElementData> OnSelectionChanged;

		public BaseListElementData this[int index] { get { return GetByIndex(index); } set { SetByIndex(index, value); } }
		public int count { get { return m_elementsData.Count; } }
		public bool multiSelection { get; set; }

		private List<BaseListElementData> m_elementsData = new List<BaseListElementData>();

		#region Add
		public void Add(BaseListElementData data)
		{
			AddElement(data);
		}

		public void Add<T>(Action<T> action) where T : BaseListElementData, new()
		{
			var data = new T();
			action(data);
			AddElement(data);
		}
		#endregion

		#region Insert
		public void Insert(int index, BaseListElementData data)
		{
			Insertlement(index, data);
		}

		public void Insert<T>(int index, Action<T> action) where T : BaseListElementData, new()
		{
			var data = new T();
			action(data);
			Insertlement(index, data);
		}
		#endregion

		#region Remove
		public void Remove(BaseListElementData data)
		{
			var index = m_elementsData.IndexOf(data);
			if (index != -1)
			{
				RemoveElementAt(index);
			}
		}

		public void RemoveAt(int index)
		{
			if ((index < 0) || (index >= m_elementsData.Count))
			{
				return;
			}

			RemoveElementAt(index);
		}

		public void RemoveAll(Predicate<BaseListElementData> predicate)
		{
			var indexes = new List<int>();
			for (int i = 0; i < m_elementsData.Count; i++)
			{
				var data = m_elementsData[i];
				if (predicate(data))
				{
					indexes.Add(i);
				}
			}
			for (int i = 0; i < indexes.Count; i++)
			{
				var index = indexes[i];
				RemoveElementAt(index);
			}
		}

		public void RemoveAll<T>(Predicate<T> predicate) where T : BaseListElementData
		{
			var indexes = new List<int>();
			for (int i = 0; i < m_elementsData.Count; i++)
			{
				var data = m_elementsData[i];
				if (predicate(data as T))
				{
					indexes.Add(i);
				}
			}
			for (int i = 0; i < indexes.Count; i++)
			{
				var index = indexes[i];
				RemoveElementAt(index);
			}
		}
		#endregion

		#region Fill
		public void Fill(int count, Func<BaseListElementData> coustructor)
		{
			ClearElements();

			for (int i = 0; i < count; i++)
			{
				var element = coustructor();
				element.OnSelectionChanged += SelectionChangeHandler;
				element.OnClick += ClickHandler;
				m_elementsData.Add(element);
			}
			ValueChanged();
		}

		public void Fill(int count, Func<int, BaseListElementData> coustructor)
		{
			ClearElements();

			for (int i = 0; i < count; i++)
			{
				var element = coustructor(i);
				element.OnSelectionChanged += SelectionChangeHandler;
				element.OnClick += ClickHandler;
				m_elementsData.Add(element);
			}
			ValueChanged();
		}

		public void Fill<T>(int count, Action<T> action) where T : BaseListElementData, new()
		{
			ClearElements();

			for (int i = 0; i < count; i++)
			{
				var element = new T();
				action(element);
				element.OnSelectionChanged += SelectionChangeHandler;
				element.OnClick += ClickHandler;
				m_elementsData.Add(element);
			}
			ValueChanged();
		}

		public void Fill<T>(int count, Action<T, int> action) where T : BaseListElementData, new()
		{
			ClearElements();

			for (int i = 0; i < count; i++)
			{
				var element = new T();
				action(element, i);
				element.OnSelectionChanged += SelectionChangeHandler;
				element.OnClick += ClickHandler;
				m_elementsData.Add(element);
			}
			ValueChanged();
		}
		#endregion

		#region Move
		public void Move(int index, int position)
		{
			if ((index < 0) || (index >= m_elementsData.Count) || (position < 0) || (position >= m_elementsData.Count))
			{
				return;
			}

			MoveElement(index, position);
		}

		public void Move(BaseListElementData data, int position)
		{
			var index = m_elementsData.IndexOf(data);
			if (index != -1)
			{
				Move(index, position);
			}
		}
		#endregion

		#region ScrollTo
		public void ScrollTo(int index)
		{
			if ((index < 0) || (index >= m_elementsData.Count))
			{
				return;
			}

			OnScrollToElement.Call(index);
		}

		public void ScrollTo(BaseListElementData data)
		{
			var index = m_elementsData.IndexOf(data);
			if (index != -1)
			{
				OnScrollToElement.Call(index);
			}
		}
		#endregion

		#region Select
		public void Selection(int index, bool selected)
		{
			if ((index < 0) || (index >= m_elementsData.Count))
			{
				return;
			}

			var data = m_elementsData[index];
			data.Selected = selected;
		}

		public void Selection(BaseListElementData data, bool selected)
		{
			if (m_elementsData.Contains(data))
			{
				data.Selected = selected;
			}
		}

		public void Selection(Predicate<BaseListElementData> predicate, bool selected)
		{
			var data = m_elementsData.Find(predicate);
			if (data != null)
			{
				data.Selected = selected;
			}
		}

		public void Selection<T>(Predicate<T> predicate, bool selected) where T : BaseListElementData
		{
			var data = Find(predicate);
			if (data != null)
			{
				data.Selected = selected;
			}
		}

		public void SelectionAll(bool selected)
		{
			for (int i = 0; i < m_elementsData.Count; i++)
			{
				var elementData = m_elementsData[i];
				elementData.Selected = selected;
			}
		}
		#endregion

		#region ForEach
		public void ForEach(Action<BaseListElementData> action)
		{
			m_elementsData.ForEach((element) => action(element));
		}

		public void ForEach(Action<BaseListElementData, int> action)
		{
			for (int i = 0; i < m_elementsData.Count; i++)
			{
				action(m_elementsData[i], i);
			}
		}

		public void ForEach<T>(Action<T> action) where T : BaseListElementData
		{
			m_elementsData.ForEach((element) => action(element as T));
		}

		public void ForEach<T>(Action<T, int> action) where T : BaseListElementData
		{
			for (int i = 0; i < m_elementsData.Count; i++)
			{
				action(m_elementsData[i] as T, i);
			}
		}
		#endregion

		#region Find
		public BaseListElementData Find(Predicate<BaseListElementData> predicate)
		{
			return m_elementsData.Find(predicate);
		}

		public T Find<T>(Predicate<T> predicate) where T : BaseListElementData
		{
			foreach (var data in m_elementsData)
			{
				var castedData = data as T;
				if (predicate(castedData))
				{
					return castedData;
				}
			}
			return null;
		}

		public List<BaseListElementData> FindAll(Predicate<BaseListElementData> predicate)
		{
			return m_elementsData.FindAll(predicate);
		}

		public List<T> FindAll<T>(Predicate<T> predicate) where T : BaseListElementData
		{
			var result = new List<T>();
			foreach (var data in m_elementsData)
			{
				var castedData = data as T;
				if (predicate(castedData))
				{
					result.Add(castedData);
				}
			}
			return result;
		}
		#endregion

		#region Exists
		public bool Exists(Predicate<BaseListElementData> predicate)
		{
			return m_elementsData.Exists(predicate);
		}

		public bool Exists<T>(Predicate<T> predicate) where T : BaseListElementData
		{
			var data = Find(predicate);
			return data != null;
		}
		#endregion

		#region Sort
		public void Sort()
		{
			m_elementsData.Sort();

			OnFullReorder.Call();
		}

		public void Sort(Comparison<BaseListElementData> comparison)
		{
			m_elementsData.Sort(comparison);

			OnFullReorder.Call();
		}
		#endregion

		#region GetSelected
		public BaseListElementData GetSelected()
		{
			return m_elementsData.Find((data) => data.Selected);
		}

		public List<BaseListElementData> GetSelectedAll()
		{
			return m_elementsData.FindAll((data) => data.Selected);
		}

		public int GetSelectedIndex()
		{
			return m_elementsData.FindIndex((data) => data.Selected);
		}
		#endregion

		#region IndexOf
		public int IndexOf(BaseListElementData data)
		{
			return m_elementsData.IndexOf(data);
		}

		public int IndexOf(Predicate<BaseListElementData> predicate)
		{
			return m_elementsData.FindIndex(predicate);
		}

		public int IndexOf<T>(Predicate<T> predicate) where T : BaseListElementData
		{
			for (int i = 0; i < m_elementsData.Count; i++)
			{
				var castedData = m_elementsData[i] as T;
				if (predicate(castedData))
				{
					return i;
				}
			}
			return -1;
		}
		#endregion

		public void Clear()
		{
			if (m_elementsData.Count > 0)
			{
				ClearElements();
				ValueChanged();
			}
		}

		protected override IEnumerable<BaseListElementData> GetValue()
		{
			return m_elementsData;
		}

		protected override void SetValue(IEnumerable<BaseListElementData> value)
		{
			var newElementsData = new List<BaseListElementData>(value);
			if ((m_elementsData.Count == 0) || (newElementsData.Count == 0))
			{
				ClearElements();
				m_elementsData = newElementsData;
				m_elementsData.ForEach((data) =>
				{
					data.OnSelectionChanged += SelectionChangeHandler;
					data.OnClick += ClickHandler;
				});
				ValueChanged();
				return;
			}

			if (newElementsData.Count > m_elementsData.Count)
			{
				for (int i = 0; i < newElementsData.Count; i++)
				{
					if (i < m_elementsData.Count)
					{
						var data = newElementsData[i];
						ChangeElement(i, data);
					}
					else
					{
						var data = newElementsData[i];
						AddElement(data);
					}
				}
			}
			else
			{
				for (int i = 0; i < newElementsData.Count; i++)
				{
					var data = newElementsData[i];
					ChangeElement(i, data);
				}

				var index = newElementsData.Count;
				while (index < m_elementsData.Count)
				{
					RemoveElementAt(index);
				}
			}
		}

		private BaseListElementData GetByIndex(int index)
		{
			return (index >= 0) && (index < m_elementsData.Count) ? m_elementsData[index] : null;
		}

		private void SetByIndex(int index, BaseListElementData data)
		{
			if ((index < 0) || (index >= m_elementsData.Count))
			{
				return;
			}

			ChangeElement(index, data);
		}

		private void MoveElement(int index, int position)
		{
			var element = m_elementsData[index];
			m_elementsData.RemoveAt(index);
			m_elementsData.Insert(position, element);

			OnElementMoved.Call(index, position);
		}

		private void AddElement(BaseListElementData data)
		{
			m_elementsData.Add(data);
			data.OnSelectionChanged += SelectionChangeHandler;
			data.OnClick += ClickHandler;

			OnElementAdded.Call(data);
		}

		private void Insertlement(int index, BaseListElementData data)
		{
			m_elementsData.Insert(index, data);
			data.OnSelectionChanged += SelectionChangeHandler;
			data.OnClick += ClickHandler;

			OnElementInserted.Call(index, data);
		}

		private void RemoveElementAt(int index)
		{
			var data = m_elementsData[index];
			data.OnSelectionChanged -= SelectionChangeHandler;
			data.OnClick -= ClickHandler;
			m_elementsData.RemoveAt(index);

			OnElementRemoved.Call(index);
		}

		private void ClearElements()
		{
			for (int i = 0; i < m_elementsData.Count; i++)
			{
				var elementData = m_elementsData[i];
				elementData.OnSelectionChanged -= SelectionChangeHandler;
				elementData.OnClick -= ClickHandler;
			}
			m_elementsData.Clear();
		}

		private void ChangeElement(int index, BaseListElementData data)
		{
			var oldData = m_elementsData[index];
			if (oldData == data)
			{
				return;
			}

			m_elementsData[index] = data;
			oldData.OnSelectionChanged -= SelectionChangeHandler;
			oldData.OnClick -= ClickHandler;
			data.OnSelectionChanged += SelectionChangeHandler;
			data.OnClick += ClickHandler;

			OnElementChanged.Call(index, data);
		}

		private void ClickHandler(BaseListElementData data)
		{
			OnClick.Call(data);
		}

		private void SelectionChangeHandler(BaseListElementData data)
		{
			if (!multiSelection && data.Selected)
			{
				for (int i = 0; i < m_elementsData.Count; i++)
				{
					var elementData = m_elementsData[i];
					if (elementData != data)
					{
						elementData.Selected = false;
					}
				}
			}
			OnSelectionChanged.Call(data);
		}
	}
}