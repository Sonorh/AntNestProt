using UnityEngine;
using UnityEngine.UI;

using UIBinding.Base;

namespace UIBinding.Components
{
	[RequireComponent(typeof(Text))]
	public class FluidIntBinding : BaseBinding<IntProperty>
	{
		[SerializeField]
		private float m_lerpTime = 2f;

		private Text m_text;
		private Outline m_outlineEffect;
		private int m_startValue;
		private int m_currentValue;
		private int m_endValue;
		private float m_lastTimeStamp;
		private bool m_isInitialValue;

		protected override void Start()
		{
			m_text = GetComponent<Text>();
			m_outlineEffect = GetComponent<Outline>();

			m_isInitialValue = true;

			base.Start();

			m_isInitialValue = false;
		}

		private void Update()
		{
			if (m_currentValue != m_endValue)
			{
				var t = Mathf.Clamp01((Time.unscaledTime - m_lastTimeStamp) / m_lerpTime);
				m_currentValue = Mathf.RoundToInt(Mathf.Lerp(m_startValue, m_endValue, Mathf.Sqrt(t)));
				m_text.text = m_currentValue.ToString();

				if (m_outlineEffect != null)
				{
					var color = m_text.color;
					color.a = 1f - t;
					m_outlineEffect.effectColor = color;
				}
			}
		}

		protected override void OnUpdateValue()
		{
			if (m_isInitialValue)
			{
				m_startValue = m_endValue;
				m_currentValue = m_endValue;
				m_endValue = property.value;
				m_text.text = m_currentValue.ToString();
			}
			else
			{
				m_isInitialValue = true;
				m_endValue = property.value;
				m_currentValue = m_endValue;
				m_startValue = m_endValue;

				m_text.text = m_endValue.ToString();
				if (m_outlineEffect != null)
				{
					m_outlineEffect.effectColor = new Color(0f, 0f, 0f, 0f);
				}
			}
			m_lastTimeStamp = Time.unscaledTime;
		}
	}
}