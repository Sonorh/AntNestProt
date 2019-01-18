using UnityEngine;
using UnityEngine.Events;
using UIBinding.Base;

namespace UIBinding.Components
{
	public class ActionBoolBinding : BaseBinding<BoolProperty>
	{
		[SerializeField]
		private bool m_invert = false;
		[SerializeField]
		private UnityEvent OnAction;

		protected override void OnUpdateValue()
		{
			bool action = (m_invert ? !property.value : property.value);

			if (action && OnAction != null)
			{
				OnAction.Invoke();
			}
		}
	}
}
