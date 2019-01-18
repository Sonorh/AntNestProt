using UIBinding.Base;
using UnityEngine;

namespace UIBinding.Components
{
	public class ScaleBinding : BaseBinding<Vector2Property>
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
			var scale = property.value;
			if (m_inited)
			{
				if (scale.x != m_rectTransform.localScale.x && scale.y != m_rectTransform.localScale.y)
				{
					m_rectTransform.localScale = new Vector3(scale.x, scale.y, m_rectTransform.localScale.z);
				}
			}
			else
			{
				if (property.value == default(Vector2))
				{
					property.value = m_rectTransform.localScale;
				}
				else if (scale.x != m_rectTransform.localScale.x && scale.y != m_rectTransform.localScale.y)
				{
					m_rectTransform.localScale = new Vector3(scale.x, scale.y, m_rectTransform.localScale.z);
				}
				m_inited = true;
			}
		}
	}
}