using UnityEngine;
using UnityEngine.UI;

using UIBinding.Base;

namespace UIBinding.Components
{
	[RequireComponent(typeof(Toggle))]
	public class ToggleIntBinding : BaseBinding<IntProperty>
	{
		[SerializeField]
		private int m_value = 0;
		[SerializeField]
		private bool m_changePropertyValue = true;

		private Toggle m_toogle;

		private void Awake()
		{
			m_toogle = GetComponent<Toggle>();
			m_toogle.onValueChanged.AddListener(ToggleValueChangeHandler);
		}

		protected override void OnUpdateValue()
		{
			m_toogle.isOn = m_value == property.value;
		}

		private void ToggleValueChangeHandler(bool value)
		{
			if (value && m_changePropertyValue)
			{
				property.value = m_value;
			}
		}
	}
}