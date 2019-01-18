using UnityEngine;
using UnityEngine.UI;
using UIBinding.Base;

namespace UIBinding.Components
{
	public class GraphicEnableBinding : BaseBinding<BoolProperty>
	{
		[SerializeField]
		private bool m_invert = false;

		private Graphic[] m_graphics;
		private int[] m_interactions;

		private void Awake()
		{
			m_graphics = GetComponentsInChildren<Graphic>(true);
			m_interactions = new int[m_graphics.Length];

			for (int i = 0; i < m_graphics.Length; i++)
			{
				m_interactions[i] = m_graphics[i].raycastTarget ? 1 : 0;
			}
		}

		protected override void OnUpdateValue()
		{
			if (m_graphics != null)
			{
				for (int i = 0; i < m_graphics.Length; i++)
				{
					bool enableFlag = (m_invert ? !property.value : property.value);

					m_graphics[i].enabled = enableFlag;

					if (m_interactions[i] == 1)
					{
						m_graphics[i].raycastTarget = enableFlag;
					}
				}

				foreach (var s in m_graphics)
				{
					s.enabled = (m_invert ? !property.value : property.value);
				}
			}
		}
	}
}
