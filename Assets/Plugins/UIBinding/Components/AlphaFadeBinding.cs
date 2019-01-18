using UnityEngine;
using UnityEngine.UI;
using System.Collections;

using UIBinding.Base;
using PM.UsefulThings.Extensions;

namespace UIBinding.Components
{
	public class AlphaFadeBinding : BaseBinding<AlphaFadeProperty>
	{
		private CanvasGroup m_canvasGroup;
		private Graphic m_graphic;
		private float m_alpha;

		private void Awake()
		{
			m_canvasGroup = GetComponent<CanvasGroup>();
			m_graphic = GetComponent<Graphic>();
		}

		private void OnDestroy()
		{
			StopAllCoroutines();
		}

		protected override void OnValueChanged()
		{
			if (this == null)
			{
				return;
			}

			StopAllCoroutines();

			if ((property.fadeTime == 0f) || property.force)
			{
				SetAlpha(property.value);
			}
			else
			{
				StartCoroutine(FadeCoroutine());
			}
		}

		protected override void OnUpdateValue()
		{
			SetAlpha(property.value);
		}

		private void SetAlpha(float alpha)
		{
			if (m_canvasGroup != null)
			{
				m_canvasGroup.alpha = alpha;
			}
			if (m_graphic != null)
			{
				m_graphic.SetAlpha(alpha);
			}
			m_alpha = alpha;
		}

		private IEnumerator FadeCoroutine()
		{
			var fade = m_alpha > property.value;
			var targetAlpha = property.value;
			var delta = Mathf.Abs(targetAlpha - m_alpha);
			while (delta > 0f)
			{
				var alpha = fade ? targetAlpha + delta : targetAlpha - delta;
				SetAlpha(alpha);

				if (property.fadeTime == 0f)
				{
					delta = 0f;
				}
				else
				{
					delta -= Time.deltaTime * (1f / property.fadeTime);
				}
				yield return null;
			}
			SetAlpha(targetAlpha);

			property.Finish();
		}
	}
}