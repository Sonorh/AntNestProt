using PM.UsefulThings.Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace UIBinding.Components
{
	[RequireComponent(typeof(Text))]
	public class LocalizationBinding : MonoBehaviour
	{
		public string key;

		[SerializeField]
		private string m_format = "{0}";

		[SerializeField]
		private bool m_forceUpperCase = true;

		private Text m_label;

		private void LocalizeText()
		{
			if (m_label != null)
			{
				m_label.text = string.Format(m_format, m_forceUpperCase ? key.Localized().ToUpper() : key.Localized());
			}
		}

		private void Awake()
		{
			m_label = GetComponent<Text>();
			LocalizationExtensions.OnLocaleChanged += OnLocalizationChanged;
		}

		private void Start()
		{
			LocalizeText();
		}

		private void OnDestroy()
		{
			LocalizationExtensions.OnLocaleChanged -= OnLocalizationChanged;
		}

		private void OnLocalizationChanged()
		{
			LocalizeText();
		}
	}
}