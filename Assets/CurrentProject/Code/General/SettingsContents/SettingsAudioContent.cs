using PM.UsefulThings;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PM.Antnest.General
{
	public class SettingsAudioContent : AbSettingContent
	{
		public Slider MasterVolume;
		public Slider MusicVolume;
		public Slider VoiceVolume;
		public Slider SfxVolume;
		public Toggle MasterToggle;
		public Toggle MusicToggle;
		public Toggle VoiceToggle;
		public Toggle SfxToggle;

		private void Start()
		{
			Title.text = "Audio";
			UpdateElements();
		}

		public override void SetDefault()
		{
			SoundManager.Instance.MasterVolume = 0.3f;
			SoundManager.Instance.MusicVolume = 1;
			SoundManager.Instance.VoiceVolume = 1;
			SoundManager.Instance.SfxVolume = 1;
			MasterToggle.isOn = true;
			MusicToggle.isOn = true;
			VoiceToggle.isOn = true;
			SfxToggle.isOn = true;
			UpdateElements();
		}

		public override void UpdateElements()
		{
			MasterVolume.value = SoundManager.Instance.MasterVolume;
			MusicVolume.value = SoundManager.Instance.MusicVolume;
			VoiceVolume.value = SoundManager.Instance.VoiceVolume;
			SfxVolume.value = SoundManager.Instance.SfxVolume;

			MasterToggle.isOn = SoundManager.Instance.IsMasterOn;
			MusicToggle.isOn = SoundManager.Instance.IsMusicOn;
			VoiceToggle.isOn = SoundManager.Instance.IsVoiceOn;
			SfxToggle.isOn = SoundManager.Instance.IsSfxOn;
		}

		public void MasterVolumeChanged(float value)
		{
			SoundManager.Instance.MasterVolume = value;
		}
		public void MusicVolumeChanged(float value)
		{
			SoundManager.Instance.MusicVolume = value;
		}
		public void VoiceVolumeChanged(float value)
		{
			SoundManager.Instance.VoiceVolume = value;
		}
		public void SfxVolumeChanged(float value)
		{
			SoundManager.Instance.SfxVolume = value;
		}

		public void MasterToggleChanged(bool value)
		{
			SoundManager.Instance.IsMasterOn = value;
		}
		public void MusicToggleChanged(bool value)
		{
			SoundManager.Instance.IsMusicOn = value;
		}
		public void VoiceToggleChanged(bool value)
		{
			SoundManager.Instance.IsVoiceOn = value;
		}
		public void SfxToggleChanged(bool value)
		{
			SoundManager.Instance.IsSfxOn = value;
		}
	}
}