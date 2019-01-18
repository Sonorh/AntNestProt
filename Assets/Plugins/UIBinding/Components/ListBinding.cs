using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

using UIBinding.Base;
using UIBinding.Elements;
using PM.UsefulThings.Extensions;

namespace UIBinding.Components
{
	public class ListBinding : BaseBinding<ListProperty>
	{
		public enum AutoScrollPivotType
		{
			LeftOrTop,
			RightOrBottom,
			Center
		}

		public event Action OnElementsCountChanged;

		[SerializeField]
		private BaseListElement m_elementPrefab = null;
		[SerializeField]
		private AutoScrollPivotType m_autoScrollPivot = AutoScrollPivotType.LeftOrTop;
		[SerializeField]
		private float m_dragTolerance = 5f;
		[SerializeField]
		private float m_scrollSpeedByDrag = 100f;
		[SerializeField]
		private float m_elementAnimationDelay = 0.5f;
		[SerializeField]
		private bool m_showOnRebuild = true;

		private List<BaseListElement> m_elements = new List<BaseListElement>();
		private ScrollRect m_scrollRect;
		private RectTransform m_content;
		private RectTransform m_viewport;
		private BaseDraggableListElement m_draggableElement;
		private RectTransform m_draggableElementBlank;
		private Vector2 m_contentStartPosition;
		private int m_draggableElementIndex;
		private bool m_canDragElements;
		private bool m_canAnimateElements;
		private bool m_scrollVertical;
		private bool m_scrollHorizontal;
		private bool m_showAnimationWasPlayed;

		private void Awake()
		{
			m_scrollRect = GetComponent<ScrollRect>();

			m_draggableElement = null;
			m_draggableElementBlank = null;
			m_draggableElementIndex = -1;
			m_canDragElements = m_elementPrefab is BaseDraggableListElement;
			m_canAnimateElements = m_elementPrefab is BaseAnimatedListElement;
			m_showAnimationWasPlayed = false;

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

		public void ShowManually()
		{
			if (!m_canAnimateElements || !gameObject.activeInHierarchy)
			{
				return;
			}

			for (int i = 0; i < m_elements.Count; i++)
			{
				var element = m_elements[i] as BaseAnimatedListElement;
				element.HideInstant();
			}
			for (int i = 0; i < m_elements.Count; i++)
			{
				var element = m_elements[i] as BaseAnimatedListElement;
				element.Show();
			}
			m_showAnimationWasPlayed = true;
		}

		public void HideManually()
		{
			if (!m_canAnimateElements || !gameObject.activeInHierarchy)
			{
				return;
			}

			for (int i = 0; i < m_elements.Count; i++)
			{
				var element = m_elements[i] as BaseAnimatedListElement;
				element.Hide();
			}
			m_showAnimationWasPlayed = false;
		}

		public void ShowManuallyForToggle(bool value)
		{
			if (value)
			{
				ShowManually();
			}
		}

		public bool IsAnyManualShowInProgress()
		{
			for (int i = 0; i < m_elements.Count; i++)
			{
				var element = m_elements[i] as BaseAnimatedListElement;
				if (element.showInProgress)
				{
					return true;
				}
			}
			return false;
		}

		public bool IsAnyManualHideInProgress()
		{
			for (int i = 0; i < m_elements.Count; i++)
			{
				var element = m_elements[i] as BaseAnimatedListElement;
				if (element.hideInProgress)
				{
					return true;
				}
			}
			return false;
		}

		private void Update()
		{
			if (m_draggableElement != null)
			{
				DragElementToMousePosition();
				MoveBlankElementIfNeeded();
				MoveContentByDragIfNeeded();
			}
		}

		protected override void OnUpdateValue()
		{
			RebuildList();
		}

		protected override void OnForceUpdate()
		{
			Unsubscribe();

			base.OnForceUpdate();

			Subscribe();
		}

		private BaseListElement InstantiateElement(BaseListElementData data, int index = -1)
		{
			var element = Instantiate(m_elementPrefab, m_content, false);
			if (index != -1)
			{
				element.transform.SetSiblingIndex(index);
			}
			element.Init(data);

			if (m_canDragElements)
			{
				var draggableElement = element as BaseDraggableListElement;
				draggableElement.OnPressed += ElementPressHandler;
				draggableElement.OnCanStartDragging += ElementCanStartDragHandler;
				draggableElement.OnCanEndDragging += ElementCanEndDragHandler;
			}
			return element;
		}

		private void DestroyElement(BaseListElement element)
		{
			if (m_canDragElements)
			{
				var draggableElement = element as BaseDraggableListElement;
				draggableElement.OnPressed -= ElementPressHandler;
				draggableElement.OnCanStartDragging -= ElementCanStartDragHandler;
				draggableElement.OnCanEndDragging -= ElementCanEndDragHandler;
			}
			Destroy(element.gameObject);
		}

		private void CreateDraggableElementBlank(BaseListElement element)
		{
			var elementTransform = element.transform as RectTransform;
			var blank = new GameObject("DraggableElementBlank");
			m_draggableElementBlank = blank.AddComponent<RectTransform>();
			m_draggableElementBlank.SetParent(m_content);
			m_draggableElementBlank.SetSiblingIndex(m_draggableElementIndex);
			m_draggableElementBlank.anchorMax = elementTransform.anchorMax;
			m_draggableElementBlank.anchorMin = elementTransform.anchorMin;
			m_draggableElementBlank.pivot = elementTransform.pivot;
			m_draggableElementBlank.sizeDelta = elementTransform.sizeDelta;
		}

		private void DragElementToMousePosition()
		{
			if (m_scrollVertical)
			{
				m_draggableElement.transform.position = new Vector3(m_draggableElement.transform.position.x, Input.mousePosition.y);
			}
			else if (m_scrollHorizontal)
			{
				m_draggableElement.transform.position = new Vector3(Input.mousePosition.x, m_draggableElement.transform.position.y);
			}
		}

		private void MoveBlankElementIfNeeded()
		{
			var blankIndex = m_draggableElementBlank.GetSiblingIndex();
			var draggablePosition = m_draggableElement.transform.position;
			for (int i = 0; i < m_content.childCount; i++)
			{
				if (i == blankIndex)
				{
					continue;
				}

				var prevPosition = m_content.GetChild(i).position;
				var nextPosition = i < m_content.childCount - 1 ? m_content.GetChild(i + 1).position : GetExtremePosition(true, m_content, 0f);

				if (i == 0)
				{
					var extremePosition = GetExtremePosition(true, m_content, 1f);
					if (ElementPositionInRange(m_scrollVertical, extremePosition, prevPosition, draggablePosition))
					{
						m_draggableElementBlank.SetSiblingIndex(0);
						break;
					}
				}
				if (ElementPositionInRange(m_scrollVertical, prevPosition, nextPosition, draggablePosition))
				{
					var index = blankIndex > i ? i + 1 : i;
					m_draggableElementBlank.SetSiblingIndex(index);
					break;
				}
			}
		}

		private void MoveContentByDragIfNeeded()
		{
			var draggablePosition = m_draggableElement.transform.position;
			if (m_scrollVertical)
			{
				var topPoint = GetExtremePosition(true, m_viewport, 1f);
				var bottomPoint = GetExtremePosition(true, m_viewport, 0f);
				if ((draggablePosition.y > topPoint.y) && (m_content.anchoredPosition.y > 0f))
				{
					m_content.position = new Vector3(m_content.position.x, m_content.position.y - m_scrollSpeedByDrag * Time.deltaTime);
				}
				if ((draggablePosition.y < bottomPoint.y) && (m_content.anchoredPosition.y < (m_content.rect.height - m_viewport.rect.height)))
				{
					m_content.position = new Vector3(m_content.position.x, m_content.position.y + m_scrollSpeedByDrag * Time.deltaTime);
				}
			}
			else if (m_scrollHorizontal)
			{

			}
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

		private void ScrollToElement(int index)
		{
			if (m_scrollRect == null)
			{
				return;
			}

			var contentSize = m_scrollRect.vertical ? m_content.rect.height : m_content.rect.width;
			var viewportSize = m_scrollRect.vertical ? m_viewport.rect.height : m_viewport.rect.width;
			if ((contentSize > viewportSize) && (index > 0))
			{
				var rectTransform = m_elements[index].transform as RectTransform;
				var elementPosition = (Vector2)m_viewport.InverseTransformPoint(GetAutoScrollPosition(rectTransform));
				var contentPosition = (Vector2)m_viewport.InverseTransformPoint(m_content.position);
				m_content.anchoredPosition = contentPosition - elementPosition;
			}
			else
			{
				m_content.anchoredPosition = Vector2.zero;
			}
			m_scrollRect.velocity = Vector2.zero;
		}

		private void GetPivotAndOffset(bool vertical, out float offset, out float pivot)
		{
			offset = 0f;
			pivot = 1f;
			switch (m_autoScrollPivot)
			{
				case AutoScrollPivotType.RightOrBottom:
					offset += vertical ? m_viewport.rect.height : m_viewport.rect.width;
					pivot = 0f;
					break;
				case AutoScrollPivotType.Center:
					offset += vertical ? m_viewport.rect.height / 2f : m_viewport.rect.width / 2f;
					pivot = 0.5f;
					break;
			}
		}

		private Vector3 GetAutoScrollPosition(RectTransform rectTransform)
		{
			float offset;
			float pivot;
			GetPivotAndOffset(m_scrollVertical, out offset, out pivot);
			return GetExtremePosition(m_scrollVertical, rectTransform, pivot, offset);
		}

		private Vector3 GetExtremePosition(bool vertical, RectTransform rectTransform, float pivot, float offset = 0f)
		{
			var position = Vector3.zero;
			if (vertical)
			{
				position.x = rectTransform.position.x;
				position.y = rectTransform.position.y + (pivot - rectTransform.pivot.y) * rectTransform.rect.height + offset;
			}
			else
			{
				position.x = rectTransform.position.x - (pivot - rectTransform.pivot.x) * rectTransform.rect.width + offset;
				position.y = rectTransform.position.y;
			}
			return position;
		}

		private void RebuildList()
		{
			Clear();

			foreach (var data in property.value)
			{
				var element = InstantiateElement(data);
				m_elements.Add(element);
			}
			TryUpdateElementsAnimation();

			OnElementsCountChanged.Call();
		}

		private void MoveElement(int index, int position)
		{
			var element = m_elements[index];
			m_elements.RemoveAt(index);
			m_elements.Insert(position, element);
			element.transform.SetSiblingIndex(position);
		}

		private void Clear()
		{
			for (int i = 0; i < m_elements.Count; i++)
			{
				DestroyElement(m_elements[i]);
			}
			m_elements.Clear();

			OnElementsCountChanged.Call();
		}

		private void TryUpdateElementsAnimation()
		{
			if (!m_canAnimateElements)
			{
				return;
			}

			var totalDuration = m_elementAnimationDelay * m_elements.Count;
			for (int i = 0; i < m_elements.Count; i++)
			{
				var element = m_elements[i] as BaseAnimatedListElement;
				var showDelay = i * m_elementAnimationDelay;
				var hideDelay = totalDuration - showDelay;
				element.SetDelays(showDelay, hideDelay);
			}
			if (m_showOnRebuild && m_showAnimationWasPlayed)
			{
				ShowManually();
			}
		}

		private void Subscribe()
		{
			property.OnElementAdded += ElementAddHandler;
			property.OnElementInserted += ElementInsertHandler;
			property.OnElementRemoved += ElementRemoveHandler;
			property.OnElementChanged += ElementChangeHandler;
			property.OnElementMoved += ElementMoveHandler;
			property.OnScrollToElement += ScrollToElementHandler;
			property.OnFullReorder += FullReorderHandler;
		}

		private void Unsubscribe()
		{
			property.OnElementAdded -= ElementAddHandler;
			property.OnElementInserted -= ElementInsertHandler;
			property.OnElementRemoved -= ElementRemoveHandler;
			property.OnElementChanged -= ElementChangeHandler;
			property.OnElementMoved -= ElementMoveHandler;
			property.OnScrollToElement -= ScrollToElementHandler;
			property.OnFullReorder -= FullReorderHandler;
		}

		private void ElementAddHandler(BaseListElementData data)
		{
			var element = InstantiateElement(data);
			m_elements.Add(element);

			TryUpdateElementsAnimation();

			OnElementsCountChanged.Call();
		}

		private void ElementInsertHandler(int index, BaseListElementData data)
		{
			var element = InstantiateElement(data, index);
			m_elements.Insert(index, element);

			TryUpdateElementsAnimation();

			OnElementsCountChanged.Call();
		}

		private void ElementRemoveHandler(int index)
		{
			var element = m_elements[index];
			m_elements.RemoveAt(index);
			DestroyElement(element);

			TryUpdateElementsAnimation();

			OnElementsCountChanged.Call();
		}

		private void ElementChangeHandler(int index, BaseListElementData data)
		{
			var element = m_elements[index];
			element.Init(data);
		}

		private void ElementMoveHandler(int index, int position)
		{
			MoveElement(index, position);

			TryUpdateElementsAnimation();
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
			m_elements.Sort();

			var index = 0;
			foreach (var data in property.value)
			{
				var element = m_elements[index];
				element.transform.SetSiblingIndex(index);
				index++;
			}
			TryUpdateElementsAnimation();
		}

		public void ElementPressHandler(BaseDraggableListElement element)
		{
			if (!m_canDragElements)
			{
				return;
			}

			m_contentStartPosition = m_content.anchoredPosition;
		}

		private void ElementCanStartDragHandler(BaseDraggableListElement element)
		{
			if (!m_canDragElements)
			{
				return;
			}

			var contentDistance = (m_contentStartPosition - m_content.anchoredPosition).magnitude;
			if (contentDistance < m_dragTolerance)
			{
				m_draggableElementIndex = element.transform.GetSiblingIndex();
				m_draggableElement = element;
				element.transform.SetParent(m_viewport);
				CreateDraggableElementBlank(element);

				element.StartDrag();
				SetScrollActive(false);
			}
		}

		private void ElementCanEndDragHandler(BaseDraggableListElement element)
		{
			if (!m_canDragElements)
			{
				return;
			}

			if (m_draggableElement != null)
			{
				var blankIndex = m_draggableElementBlank.GetSiblingIndex();
				Destroy(m_draggableElementBlank.gameObject);
				m_draggableElementBlank = null;

				m_draggableElement.transform.SetParent(m_content);
				m_draggableElement = null;

				property.Move(element.data, blankIndex);

				element.EndDrag();
				SetScrollActive(true);
			}
		}
	}
}