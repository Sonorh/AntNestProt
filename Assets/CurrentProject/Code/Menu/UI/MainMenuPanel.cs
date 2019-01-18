using PM.UsefulThings;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace PM.Antnest.Menu
{
	public class MainMenuPanel : AbPanel
	{
		public override bool IsSolid => true;

		public void StartTutorial()
		{
			MainMenuManager.Instance.StartTutorial();
		}

		public void NewGame()
		{
			MainMenuManager.Instance.NewGame();
		}

		public void LoadGame()
		{
			MainMenuManager.Instance.LoadGame();
		}

		public void Settings()
		{
			MainMenuManager.Instance.Settings();
		}

		public void Credits()
		{
			MainMenuManager.Instance.Credits();
		}

		public void Quit()
		{
			MainMenuManager.Instance.Quit();
		}
	}
}