using UnityEngine;
using UnityEngine.UI;

using UIBinding.Base;

namespace UIBinding.Components
{
	[RequireComponent(typeof(RawImage))]
	public class RawImageTextureBinding : BaseBinding<TextureProperty>
	{
		private RawImage m_component;

		private void Awake()
		{
			m_component = GetComponent<RawImage>();
		}

		protected override void OnUpdateValue()
		{
			m_component.texture = property.value;
		}
	}
}
