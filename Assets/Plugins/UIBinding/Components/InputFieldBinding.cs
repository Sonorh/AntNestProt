using UnityEngine;
using UnityEngine.UI;
using UIBinding.Base;

namespace UIBinding.Components
{
	[RequireComponent(typeof(InputField))]
	public class InputFieldBinding : BaseBinding<StringProperty>
	{
		[SerializeField]
		private bool m_changePropertyValue = true;

		private InputField m_component;

		private void Awake()
		{
			m_component = GetComponent<InputField>();
			m_component.onValueChanged.AddListener(ValueChangeHandler);
		}

		protected override void OnUpdateValue()
		{
			if (m_component.text != property.value)
			{
				m_component.text = property.value;
			}
		}

		private void ValueChangeHandler(string value)
		{
			if (m_changePropertyValue)
			{
				property.value = value;
			}
		}
	}
}