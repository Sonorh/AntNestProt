using UnityEngine;
using UnityEngine.UI;

using UIBinding.Base;

namespace UIBinding.Components
{
	public class LayoutBinding : BaseBinding<FloatProperty>
	{
		public enum ParameterType
		{
			Spacing,
			MinWidth,
			MinHeight,
			PreferedWidth,
			PreferedHeight,
			FlexibleWidth,
			FlexibleHeight
		}

		[SerializeField]
		private ParameterType m_parameterType = ParameterType.Spacing;

		private HorizontalOrVerticalLayoutGroup m_group;
		private LayoutElement m_element;

		private void Awake()
		{
			m_group = GetComponent<HorizontalOrVerticalLayoutGroup>();
			m_element = GetComponent<LayoutElement>();
		}

		protected override void OnUpdateValue()
		{
			switch(m_parameterType)
			{
				case ParameterType.Spacing:
					if(m_group != null)
					{
						m_group.spacing = property.value;
					}
					break;
				case ParameterType.MinWidth:
					if(m_element != null)
					{
						m_element.minWidth = property.value;
					}
					break;
				case ParameterType.MinHeight:
					if(m_element != null)
					{
						m_element.minHeight = property.value;
					}
					break;
				case ParameterType.PreferedWidth:
					if(m_element != null)
					{
						m_element.preferredWidth = property.value;
					}
					break;
				case ParameterType.PreferedHeight:
					if(m_element != null)
					{
						m_element.preferredHeight = property.value;
					}
					break;
				case ParameterType.FlexibleWidth:
					if(m_element != null)
					{
						m_element.flexibleWidth = property.value;
					}
					break;
				case ParameterType.FlexibleHeight:
					if(m_element != null)
					{
						m_element.flexibleHeight = property.value;
					}
					break;
			}
		}
	}
}