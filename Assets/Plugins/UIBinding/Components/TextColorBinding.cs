using UnityEngine;
using UnityEngine.UI;

using UIBinding.Base;
using PM.UsefulThings.Extensions;

namespace UIBinding.Components
{
	[RequireComponent(typeof(Text))]
	public class TextColorBinding : BaseBinding<ColorProperty>
	{
		private Text m_text;

		private void Awake()
		{
			m_text = GetComponent<Text>();
		}

		protected override void OnUpdateValue()
		{
			m_text.SetColorOnly(property.value);
		}
	}
}
