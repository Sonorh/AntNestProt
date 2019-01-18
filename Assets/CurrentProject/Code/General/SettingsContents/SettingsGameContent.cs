using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PM.Antnest.General
{
	public class SettingsGameContent : AbSettingContent
	{
		public Toggle FullscreenToggle;
		public Dropdown ResolutionDrop;
		public Dropdown QualityDrop;

		private void Start()
		{
			Title.text = "Game";
		}

		public override void SetDefault()
		{
			PlayerPrefs.SetInt("Quality", 1);
			UpdateElements();
		}
		public override void UpdateElements()
		{
			FullscreenToggle.isOn = Screen.fullScreen;
			var res = PlayerPrefs.GetInt("Res");

			switch (res)
			{
				case 0:
					ResolutionDrop.value = 0;
					break;
				case 1:
					ResolutionDrop.value = 1;
					break;
				case 2:
					ResolutionDrop.value = 2;
					break;
				default:
					ResolutionDrop.value = 0;
					break;
			}
			
		}

		public void FullscreenChanged(bool value)
		{
			Screen.fullScreen = value;
		}

		public void ResolutionChanged(int value)
		{
			switch(value)
			{
				case 0:
					Screen.SetResolution(1920, 1080, Screen.fullScreen);
					break;
				case 1:
					Screen.SetResolution(1024, 768, Screen.fullScreen);
					break;
				case 2:
					Screen.SetResolution(2200, 1000, Screen.fullScreen);
					break;
				default:
					Screen.SetResolution(1920, 1080, Screen.fullScreen);
					break;
			}

			PlayerPrefs.SetInt("Res", value);
		}
	}
}