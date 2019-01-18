using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

using UIBinding.Base;
using UIBinding.Elements;
using PM.UsefulThings.Extensions;

namespace UIBinding.Components
{
	public class InfinityListBinding : BaseBinding<ListProperty>
	{
		public enum ScrollType
		{
			Vertical,
			Horizontal
		}

		public event Action onElementsCountChanged;

		[SerializeField]
		private ScrollType m_scrollType = ScrollType.Vertical;
		[SerializeField]
		private BaseListElement m_elementPrefab = null;
		[SerializeField]
		private float m_spacing;

		private BaseListElement[] m_links = new BaseListElement[0];
		private List<BaseListElement> m_pool = new List<BaseListElement>();
		private ScrollRect m_scrollRect;
		private RectTransform m_content;
		private RectTransform m_viewport;
		private bool m_scrollVertical;
		private bool m_scrollHorizontal;
		private float m_itemSize;
		private int m_lastScrollIndex;

		protected void Awake()
		{
			m_scrollRect = GetComponent<ScrollRect>();

			if (m_scrollRect != null)
			{
				m_content = m_scrollRect.content != null ? m_scrollRect.content : transform as RectTransform;
				m_viewport = m_scrollRect.viewport != null ? m_scrollRect.viewport : transform as RectTransform;
				m_scrollVertical = m_scrollRect.vertical;
				m_scrollHorizontal = m_scrollRect.horizontal;
			}
			else
			{
				m_content = transform as RectTransform;
				m_viewport = transform as RectTransform;
				m_scrollVertical = false;
				m_scrollHorizontal = false;
			}

			if (m_scrollType == ScrollType.Vertical)
			{
				m_itemSize = m_elementPrefab.GetComponent<RectTransform>().GetHeight();
			}
			else
			{
				m_itemSize = m_elementPrefab.GetComponent<RectTransform>().GetWidth();
			}

			m_itemSize = m_itemSize > float.Epsilon ? m_itemSize : 1f;
		}

		protected override void Start()
		{
			base.Start();

			Subscribe();
		}

		private void OnDestroy()
		{
			if (property != null)
			{
				Unsubscribe();
			}
		}

		protected override void OnUpdateValue()
		{
			RebuildList();
		}

		private bool ElementPositionInRange(bool vertical, Vector3 prevPosition, Vector3 nextPosition, Vector3 position)
		{
			return vertical ? (prevPosition.y > position.y) && (position.y > nextPosition.y) : (prevPosition.x > position.x) && (position.x > nextPosition.x);
		}

		private void SetScrollActive(bool active)
		{
			if (active)
			{
				m_scrollRect.vertical = m_scrollVertical;
				m_scrollRect.horizontal = m_scrollHorizontal;
			}
			else
			{
				m_scrollRect.vertical = false;
				m_scrollRect.horizontal = false;
			}
		}

		protected void ScrollToElement(int index)
		{
			if (m_scrollRect == null)
			{
				return;
			}

			var contentSize = m_scrollRect.vertical ? m_content.rect.height : m_content.rect.width;
			var viewportSize = m_scrollRect.vertical ? m_viewport.rect.height : m_viewport.rect.width;
			if (contentSize > viewportSize && index > 0)
			{
				var idx = Mathf.Min(index, Mathf.Max(0, property.count - m_pool.Count + 1));
				if (m_scrollVertical)
				{
					m_content.anchoredPosition = new Vector2(0, m_itemSize * idx + m_spacing * Mathf.Max(0, idx));
				}
				else
				{
					m_content.anchoredPosition = new Vector2(m_itemSize * idx + m_spacing * Mathf.Max(0, idx), 0);
				}
			}
			else
			{
				m_content.anchoredPosition = Vector2.zero;
			}
			m_scrollRect.velocity = Vector2.zero;
		}

		private void RebuildList()
		{
			var count = 0;
			if (m_scrollType == ScrollType.Vertical)
			{
				count = Mathf.Min(property.count, Mathf.RoundToInt(m_viewport.GetHeight() / m_itemSize) + 2);
			}
			else
			{
				count = Mathf.Min(property.count, Mathf.RoundToInt(m_viewport.GetWidth() / m_itemSize) + 2);
			}

			if (m_pool.Count > count)
			{
				var removeCount = m_pool.Count - count;
				for (int i = 0; i < removeCount; i++)
				{
					var obj = m_pool[m_pool.Count - i - 1];
					if (obj != null)
					{
						Destroy(obj.gameObject);
					}
				}

				m_pool.RemoveRange(m_pool.Count - removeCount, removeCount);
			}
			else
			{
				var addCount = count - m_pool.Count;
				for (int i = 0; i < addCount; i++)
				{
					m_pool.Add(Instantiate(m_elementPrefab, m_content, false));
				}
			}

			if (m_scrollType == ScrollType.Vertical)
			{
				int totalHeight = (int)(property.count * m_itemSize + m_spacing * (Mathf.Max(0, property.count - 1)));
				m_content.SetHeight(totalHeight);
				var p = m_content.anchoredPosition;
				m_content.anchoredPosition = new Vector2(p.x, Mathf.Clamp(p.y, - totalHeight, 0f));
			}
			else
			{
				int totalWidth = (int)(property.count * m_itemSize + m_spacing * (Mathf.Max(0, property.count - 1)));
				m_content.SetWidth(totalWidth);
				var p = m_content.anchoredPosition;
				m_content.anchoredPosition = new Vector2(Mathf.Clamp(p.x, - totalWidth, 0f), p.y);
			}

			if (count == 0)
			{
				return;
			}

			m_links = new BaseListElement[property.count];

			var index = GetCurrentIndex();
			for (int i = 0; i < m_pool.Count; i++)
			{
				var obj = m_pool[i];
#if TEST_BUILD
				obj.name = "Item_" + index + i;
#endif
				obj.transform.SetParent(m_content.transform, false);
				obj.transform.localScale = Vector3.one;

				if (m_scrollType == ScrollType.Vertical)
				{
					RectTransform rect = obj.GetComponent<RectTransform>();
					if (rect != null)
					{
						Vector2 anchor = rect.pivot;
						rect.pivot = new Vector2(anchor.x, 1);
					}
				}
				else
				if (m_scrollType == ScrollType.Horizontal)
				{
					RectTransform rect = obj.GetComponent<RectTransform>();
					if (rect != null)
					{
						Vector2 anchor = rect.pivot;
						rect.pivot = new Vector2(0, anchor.y);
					}
				}

				UpdateItem(m_pool[i], index + i);
				m_links[index + i] = obj;
			}

			onElementsCountChanged.Call();
		}

		private Vector3 GetLocationAppear(Vector2 localPos, int index)
		{
			Vector3 vec = localPos;
			if (m_scrollType == ScrollType.Vertical)
			{
				vec = new Vector3(vec.x, -m_itemSize * index - m_spacing * (Mathf.Max(0, index)), 0);
			}
			else
			{
				vec = new Vector3(m_itemSize * index + m_spacing * (Mathf.Max(0, index)), vec.y, 0);
			}

			return vec;
		}

		private void UpdateItem(BaseListElement poolPrefab, int index)
		{
			var vec = GetLocationAppear(poolPrefab.transform.localPosition, index);
			poolPrefab.transform.localPosition = vec;
#if TEST_BUILD
			poolPrefab.name = "Item_" + index;
#endif
			poolPrefab.Init(property[index]);
		}

		private int GetCurrentIndex()
		{
			int index = -1;
			if (m_scrollType == ScrollType.Vertical)
			{
				index = (int)(m_content.anchoredPosition.y / m_itemSize);
			}
			else
			{
				index = (int)(-m_content.anchoredPosition.x / m_itemSize);
			}

			return Mathf.Clamp(index, 0, property.count - m_pool.Count);
		}

		private void MoveElement(int index, int position)
		{
			RebuildList();
		}

		private void Subscribe()
		{
			property.OnElementAdded += ElementAddHandler;
			property.OnElementRemoved += ElementRemoveHandler;
			property.OnElementChanged += ElementChangeHandler;
			property.OnElementMoved += ElementMoveHandler;
			property.OnScrollToElement += ScrollToElementHandler;
			property.OnFullReorder += FullReorderHandler;

			m_scrollRect.onValueChanged.AddListener(OnScrollChange);
		}

		private void Unsubscribe()
		{
			property.OnElementAdded -= ElementAddHandler;
			property.OnElementRemoved -= ElementRemoveHandler;
			property.OnElementChanged -= ElementChangeHandler;
			property.OnElementMoved -= ElementMoveHandler;
			property.OnScrollToElement -= ScrollToElementHandler;
			property.OnFullReorder -= FullReorderHandler;

			m_scrollRect.onValueChanged.RemoveListener(OnScrollChange);
		}

		private void ElementAddHandler(BaseListElementData data)
		{
			RebuildList();

			onElementsCountChanged.Call();
		}

		private void ElementRemoveHandler(int index)
		{
			RebuildList();

			onElementsCountChanged.Call();
		}

		private void ElementChangeHandler(int index, BaseListElementData data)
		{
			var currentIndex = GetCurrentIndex();
			if (index >= currentIndex && index < currentIndex + m_pool.Count)
			{
				UpdateItem(m_pool[index - currentIndex], index);
			}
		}

		private void ElementMoveHandler(int index, int position)
		{
			MoveElement(index, position);
		}

		private void ScrollToElementHandler(int index)
		{
			if (m_scrollRect != null)
			{
				ScrollToElement(index);
			}
		}

		private void FullReorderHandler()
		{
			RebuildList();
		}

		public void OnScrollChange(Vector2 delta)
		{
			if (property.count < 1)
			{
				return;
			}

			int index = GetCurrentIndex();
			if (m_lastScrollIndex != index)
			{
				m_lastScrollIndex = index;

				if (!NeedFullUpdate(index))
				{
					var objIndex = m_links[index];
					if (objIndex == null)
					{
						int next = index + m_pool.Count;
						if (next > property.count - 1)
						{
							return;
						}
						else
						{
							var objNow = m_links[next];
							if (objNow != null)
							{
								m_links[next] = objIndex;
								m_links[index] = objNow;
								UpdateItem(m_links[index], index);
							}
						}
					}
					else
					{
						if (index > 0)
						{
							var obj = m_links[index - 1];
							if (obj == null)
							{
								return;
							}
							int next = index - 1 + m_pool.Count;
							if (next > property.count - 1)
							{
								return;
							}
							else
							{
								var objNow = m_links[next];
								if (objNow == null)
								{
									m_links[next] = obj;
									m_links[index - 1] = objNow;
									UpdateItem(m_links[next], next);
								}
							}
						}
					}
				}
			}
		}

		public bool NeedFullUpdate(int index)
		{
			bool isNeedFix = false;

			int add = index + 1;
			for (int i = add; i < add + m_pool.Count - 2; i++)
			{
				if (i < property.count)
				{
					var obj = m_links[i];
					if (obj == null)
					{
						isNeedFix = true;
						break;
					}
					else
					if (obj.data != property[i])
					{
						isNeedFix = true;
						break;
					}
				}
			}

			if (isNeedFix)
			{
				for (int i = 0; i < property.count; i++)
				{
					m_links[i] = null;
				}

				int start = index;
				if (start + m_pool.Count > property.count)
				{
					start = property.count - m_pool.Count;
				}

				for (int i = 0; i < m_pool.Count; i++)
				{
					m_links[start + i] = m_pool[i];
					UpdateItem(m_links[start + i], start + i);
				}
				return true;
			}

			return false;
		}

		/*
		public void OnScrollChange(Vector2 vec)
		{
			if (m_pool.Count < 1)
			{
				return;
			}

			int index = GetCurrentIndex();
			if (m_cacheOld != index)
			{
				m_cacheOld = index;
			}
			else
			{
				return;
			}

			for (int i = 0, count = m_pool.Count; i < count; i++)
			{
				var abs = index + i;
				if (abs < property.count)
				{
					UpdateItem(m_pool[i], abs);
				}
			}
		}
		*/
	}
}