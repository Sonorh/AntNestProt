using UnityEngine;
using UnityEngine.UI;

using UIBinding.Base;

namespace UIBinding.Components
{
	[RequireComponent(typeof(Graphic))]
	public class GraphicFixedColorBoolBinding : BaseBinding<BoolProperty>
	{
		[SerializeField]
		private Color m_colorOnTrue = Color.white;
		[SerializeField]
		private Color m_colorOnFalse = Color.white;

		private Graphic m_graphic;

		private void Awake()
		{
			m_graphic = GetComponent<Graphic>();
		}

		protected override void OnUpdateValue()
		{
			if (property.value)
			{
				m_graphic.color = m_colorOnTrue;
			}
			else
			{
				m_graphic.color = m_colorOnFalse;
			}
		}
	}
}

