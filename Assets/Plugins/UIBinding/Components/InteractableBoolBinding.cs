using UnityEngine;

using UIBinding.Base;
using UnityEngine.UI;

namespace UIBinding.Components
{
	public class InteractableBoolBinding : BaseBinding<BoolProperty>
	{
		[SerializeField]
		private bool m_invert = false;

		private Selectable m_component;

		private void Awake()
		{
			m_component = GetComponent<Selectable>();
		}

		protected override void OnUpdateValue()
		{
			m_component.interactable = m_invert ? !property.value : property.value;
		}
	}
}
