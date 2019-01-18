using UIBinding.Base;
using UnityEngine;

namespace UIBinding.Components
{
	public class RectSizeBinding : BaseBinding<Vector2Property>
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
			var sizeDelta = property.value;
			if (m_inited)
			{
				if (sizeDelta != m_rectTransform.sizeDelta)
				{
					m_rectTransform.sizeDelta = sizeDelta;
				}
			}
			else
			{
				if (property.value == default(Vector2))
				{
					property.value = m_rectTransform.sizeDelta;
				}
				else if (sizeDelta != m_rectTransform.sizeDelta)
				{
					m_rectTransform.sizeDelta = sizeDelta;
				}
				m_inited = true;
			}
		}
	}
}