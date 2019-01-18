using UnityEngine;
using UnityEngine.UI;

using UIBinding.Base;

namespace UIBinding.Components
{
	[RequireComponent(typeof(Image))]
	public class ImageFillBinding : BaseBinding<FloatProperty>
	{
		private Image m_component;

		[SerializeField]
		private float m_max = 100f;

		private void Awake()
		{
			m_component = GetComponent<Image>();
		}

		protected override void OnUpdateValue()
		{
			m_component.fillAmount = property.value / m_max;
		}
	}
}