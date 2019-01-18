using UnityEngine;
using UnityEngine.UI;
using UIBinding.Base;
using System.Collections.Generic;

namespace UIBinding.Components
{
	[RequireComponent(typeof(Dropdown))]
	public class DropdownListBinding : BaseBinding<EnumerableProperty>
	{
		private Dropdown m_component;

		private void Awake()
		{
			m_component = GetComponent<Dropdown>();
		}

		protected override void OnUpdateValue()
		{
			var options = new List<Dropdown.OptionData>();

			foreach (var item in property.value)
			{
				options.Add(new Dropdown.OptionData(item.ToString()));
			}

			m_component.options = options;
		}
	}
}