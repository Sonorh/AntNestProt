using UnityEngine;
using UnityEngine.UI;
using UIBinding.Base;
using PM.UsefulThings.Extensions;

namespace UIBinding.Components
{
	[RequireComponent(typeof(Text))]
	public class TextCSSBinding : BaseBinding<ColorStrStrProperty>
	{
		private enum CSSComponents { Color, Str1, Str2 }

		private Text m_text;

		[SerializeField]
		private CSSComponents m_elementType;

		[SerializeField]
		private string m_format = "{0}";

		private void Awake()
		{
			m_text = GetComponent<Text>();
		}

		protected override void OnUpdateValue()
		{
			if (m_elementType == CSSComponents.Color)
			{
				if (property.value != null)
				{
					m_text.SetColorOnly(property.value.color);
				}
			}
			else if (m_elementType == CSSComponents.Str1)
			{
				if (property.value != null)
				{
					m_text.text = string.Format(m_format, property.value.str1);
				}
				else
				{
					m_text.text = null;
				}
			}
			else
			{
				if (property.value != null)
				{
					m_text.text = string.Format(m_format, property.value.str2);
				}
				else
				{
					m_text.text = null;
				}
			}
		}
	}
}
