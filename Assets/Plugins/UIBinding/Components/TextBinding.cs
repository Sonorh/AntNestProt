using UnityEngine;
using UnityEngine.UI;

using UIBinding.Base;

namespace UIBinding.Components
{
	public class TextBinding : BaseBinding<Property>
	{
		private Text m_text;
		private InputField m_inputField;

		[SerializeField]
		private string m_format = "{0}";

		[SerializeField]
		private bool m_forceUpperCase = false;

		private void Awake()
		{
			m_text = GetComponent<Text>();

			if (m_text == null)
			{
				m_inputField = GetComponent<InputField>();
			}
		}

		protected override void OnUpdateValue()
		{
			var val = property.ValueToString();
			if (!string.IsNullOrEmpty(val) && m_forceUpperCase)
			{
				val = val.ToUpper();
			}

			if (m_text != null)
			{
				m_text.text = string.Format(m_format, val);
			}
			else
			if (m_inputField != null)
			{
				m_inputField.text = string.Format(m_format, val);
			}
		}
	}
}