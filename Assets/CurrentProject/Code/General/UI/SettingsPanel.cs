using PM.UsefulThings;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PM.Antnest.General
{
	public class SettingsPanel : AbPanel
	{
		public AbSettingContent GameSettings;
		public AbSettingContent AudioSettings;
		public AbSettingContent ControlsSettings;

		private void Awake()
		{
			ShowGameSettings();
		}

		public void SaveAndClose()
		{
			Close();
		}

		public void SetDefault()
		{
			GameSettings.SetDefault();
			AudioSettings.SetDefault();
			ControlsSettings.SetDefault();
		}

		public void ShowGameSettings()
		{
			HideEverything();

			GameSettings.gameObject.SetActive(true);
		}

		public void ShowAudioSettings()
		{
			HideEverything();

			AudioSettings.gameObject.SetActive(true);
		}

		public void ShowControlsSettings()
		{
			HideEverything();

			ControlsSettings.gameObject.SetActive(true);
		}

		private void HideEverything()
		{
			GameSettings.gameObject.SetActive(false);
			AudioSettings.gameObject.SetActive(false);
			ControlsSettings.gameObject.SetActive(false);
		}
	}
}