using UnityEngine;

using UIBinding.Base;

namespace UIBinding.Components
{
	public class VisibilityNullEmptyBinding : BaseBinding<Property>
	{
		[SerializeField]
		private bool m_invert = false;

		protected override void OnUpdateValue()
		{
			gameObject.SetActive(m_invert ? string.IsNullOrEmpty(property.ValueToString()) : !string.IsNullOrEmpty(property.ValueToString()));
		}
	}
}