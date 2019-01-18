using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PM.Antnest.General
{
	public class SettingsControlsContent : AbSettingContent
	{
		public Text MenuBtnText;

		private void Start()
		{
			Title.text = "Controls";
		}

		private void OnDisable()
		{
			UpdateElements();
		}

		public override void SetDefault()
		{
			PlayerPrefs.SetInt("Esc", (int)KeyCode.Escape);

			UpdateElements();
		}

		public override void UpdateElements()
		{
			StopAllCoroutines();

			MenuBtnText.text = ((KeyCode)PlayerPrefs.GetInt("Esc")).ToString();
		}

		public void OnEscChanged()
		{
			StopAllCoroutines();
			UpdateElements();

			MenuBtnText.text = "Reading";
			StartCoroutine(ReadKey());
		}

		private IEnumerator ReadKey()
		{
			while (Input.GetMouseButtonDown(0) == false)
			{
				foreach (KeyCode kcode in System.Enum.GetValues(typeof(KeyCode)))
				{
					if (Input.GetKeyDown(kcode))
					{
						PlayerPrefs.SetInt("Esc", (int)kcode);
						UpdateElements();
						yield break;
					}
				}
				yield return new WaitForEndOfFrame();
			}
		}
	}
}