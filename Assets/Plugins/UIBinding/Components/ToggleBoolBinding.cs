using UnityEngine;
using UnityEngine.UI;

using UIBinding.Base;

namespace UIBinding.Components
{
	[RequireComponent(typeof(Toggle))]
	public class ToggleBoolBinding : BaseBinding<BoolProperty>
	{
		[SerializeField]
		private bool m_changePropertyValue = true;
		[SerializeField]
		private bool m_invert = false;

		private Toggle m_toogle;

		private void Awake()
		{
			m_toogle = GetComponent<Toggle>();
			m_toogle.onValueChanged.AddListener(ToggleValueChangeHandler);
		}

		protected override void OnUpdateValue()
		{
			m_toogle.isOn = m_invert ? !property.value : property.value;
		}

		private void ToggleValueChangeHandler(bool value)
		{
			if (m_changePropertyValue)
			{
				property.value = value;
			}
		}
	}
}