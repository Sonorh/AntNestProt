using UnityEngine;
using UnityEngine.UI;

using UIBinding.Base;

namespace UIBinding.Components
{
	[RequireComponent(typeof(RawImage))]
	public class RawImageMaterialBinding : BaseBinding<MaterialProperty>
	{
		private RawImage m_component;

		private void Awake()
		{
			m_component = GetComponent<RawImage>();
		}

		protected override void OnUpdateValue()
		{
			m_component.material = property.value;
		}
	}
}
