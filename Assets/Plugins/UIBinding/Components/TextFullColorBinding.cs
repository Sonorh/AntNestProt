using UnityEngine;
using UnityEngine.UI;

using UIBinding.Base;
using PM.UsefulThings.Extensions;

namespace UIBinding.Components
{
	[RequireComponent(typeof(Text))]
	public class TextFullColorBinding : BaseBinding<ColorProperty>
	{
		private Text m_text;

		private void Awake()
		{
			m_text = GetComponent<Text>();
		}

		protected override void OnUpdateValue()
		{
			var alpha = property.value.a;

			m_text.SetColorOnly(property.value);
			m_text.SetAlpha(alpha);
		}
	}
}

