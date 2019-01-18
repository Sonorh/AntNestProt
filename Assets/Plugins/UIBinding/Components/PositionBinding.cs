using UIBinding.Base;
using UnityEngine;

namespace UIBinding.Components
{
	public class PositionBinding : BaseBinding<Vector2Property>
	{
		private RectTransform m_rectTransform;
		private bool m_inited;

		private void Awake()
		{
			m_rectTransform = transform as RectTransform;
			m_inited = false;
		}

		protected override void OnUpdateValue()
		{
			var anchoredPosition = property.value;
			if (m_inited)
			{
				if (anchoredPosition != m_rectTransform.anchoredPosition)
				{
					m_rectTransform.anchoredPosition = anchoredPosition;
				}
			}
			else
			{
				if (property.value == default(Vector2))
				{
					property.value = m_rectTransform.anchoredPosition;
				}
				else if (anchoredPosition != m_rectTransform.anchoredPosition)
				{
					m_rectTransform.anchoredPosition = anchoredPosition;
				}
				m_inited = true;
			}
		}
	}
}