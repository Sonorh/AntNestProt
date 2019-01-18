using UnityEngine;
using UnityEngine.UI;

using UIBinding.Base;

namespace UIBinding.Components
{
	[RequireComponent(typeof(SpriteRenderer))]
	public class SpriteColorBinding : BaseBinding<ColorProperty>
	{
		private SpriteRenderer m_spriteRen;

		private void Awake()
		{
			m_spriteRen = GetComponent<SpriteRenderer>();
		}

		protected override void OnUpdateValue()
		{
			m_spriteRen.color = property.value;
		}
	}
}