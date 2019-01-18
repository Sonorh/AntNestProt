using UnityEngine;
using UnityEngine.UI;

using UIBinding.Base;

namespace UIBinding.Components
{
	[RequireComponent(typeof(Graphic))]
	public class ColorChannelBinding : BaseBinding<FloatProperty>
	{
		public enum ChannelType
		{
			Red,
			Green,
			Blue,
			Alpha
		}

		[SerializeField]
		private ChannelType m_channel;

		private Graphic m_graphic;

		private void Awake()
		{
			m_graphic = GetComponent<Graphic>();
		}

		protected override void OnUpdateValue()
		{
			var color = m_graphic.color;
			switch (m_channel)
			{
				case ChannelType.Red:
					color.r = property.value;
					break;
				case ChannelType.Green:
					color.g = property.value;
					break;
				case ChannelType.Blue:
					color.b = property.value;
					break;
				case ChannelType.Alpha:
					color.a = property.value;
					break;
			}
			m_graphic.color = color;
		}
	}
}