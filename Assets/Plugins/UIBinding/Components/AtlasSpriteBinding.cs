using UnityEngine;
using UnityEngine.UI;

using UIBinding.Base;
using UnityEngine.U2D;

namespace UIBinding.Components
{
	[RequireComponent(typeof(Image))]
	public class AtlasSpriteBinding : BaseBinding<StringProperty>
	{
		public SpriteAtlas[] m_atlases;

		private Image m_component;

		private void Awake()
		{
			m_component = GetComponent<Image>();
		}

		protected override void OnUpdateValue()
		{
			Sprite sprite = null;
			for(int i = 0; i < m_atlases.Length; i++)
			{
				var atlas = m_atlases[i];
				if (atlas != null)
				{
					sprite = atlas.GetSprite(property.value);
					if (sprite != null)
					{
						break;
					}
				}
			}
			m_component.sprite = sprite;
		}
	}
}
