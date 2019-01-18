using UnityEngine;
using UnityEngine.UI;
using UIBinding.Base;

namespace UIBinding.Components
{
	[RequireComponent(typeof(Dropdown))]
	public class DropdownBinding : BaseBinding<IntProperty>
	{
		[SerializeField]
		private bool m_changePropertyValue = true;

		private Dropdown m_component;

		private void Awake()
		{
			m_component = GetComponent<Dropdown>();
			m_component.onValueChanged.AddListener(ValueChangeHandler);
		}

		protected override void OnUpdateValue()
		{
			if (m_component.value != property.value)
			{
				m_component.value = property.value;
			}
		}

		private void ValueChangeHandler(int value)
		{
			if (m_changePropertyValue)
			{
				property.value = value;
			}
		}
	}
}