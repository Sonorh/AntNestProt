using UnityEngine;
using UnityEngine.UI;

using UIBinding.Base;

namespace UIBinding.Components
{
	[RequireComponent(typeof(Graphic))]
	public class ColorBinding : BaseBinding<ColorProperty>
	{
		private Graphic m_graphic;

		private void Awake()
		{
			m_graphic = GetComponent<Graphic>();
		}

		protected override void OnUpdateValue()
		{
			m_graphic.color = property.value;
		}
	}
}