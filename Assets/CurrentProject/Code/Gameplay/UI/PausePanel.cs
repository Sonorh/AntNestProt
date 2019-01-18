using PM.Antnest.General;
using PM.UsefulThings;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PM.Antnest.Gameplay
{
	public class PausePanel : AbPanel
	{
		public override bool IsSolid => true;

		public void Back()
		{
			GameplayManager.Instance.Unpause();
		}

		public void Settings()
		{
			WindowManagerUT.Instance.OpenNewPanel<SettingsPanel>(WindowCloseModes.CloseNonSolid);
		}

		public void MainMenu()
		{
			if (GameplayManager.HasInstance)
			{
				GameplayManager.Instance.GoToMainMenu();
			}
			else
			{
				SceneManager.LoadScene("MainMenu");
			}
		}
	}
}