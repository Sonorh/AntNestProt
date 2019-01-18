using UnityEngine;
using UnityEngine.UI;

using UIBinding.Base;

namespace UIBinding.Components
{
	[RequireComponent(typeof(Scrollbar))]
	public class ScrollBarBinding : BaseBinding<FloatProperty>
	{
		private Scrollbar m_scrollBar;

		private void Awake()
		{
			m_scrollBar = GetComponent<Scrollbar>();
			m_scrollBar.onValueChanged.AddListener(ScrollValueChangeHandler);
		}

		protected override void OnUpdateValue()
		{
			m_scrollBar.value = property.value;
		}

		private void ScrollValueChangeHandler(float value)
		{
			property.value = value;
		}
	}
}