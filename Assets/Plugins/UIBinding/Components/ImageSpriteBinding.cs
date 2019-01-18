using UnityEngine;
using UnityEngine.UI;

using UIBinding.Base;

namespace UIBinding.Components
{
	[RequireComponent(typeof(Image))]
	public class ImageSpriteBinding : BaseBinding<SpriteProperty>
	{
		private Image m_component;

		private void Awake()
		{
			m_component = GetComponent<Image>();
		}

		protected override void OnUpdateValue()
		{
			m_component.sprite = property.value;
		}
	}
}
