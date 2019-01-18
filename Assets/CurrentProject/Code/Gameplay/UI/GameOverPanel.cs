using PM.Antnest.General;
using PM.UsefulThings;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PM.Antnest.Gameplay
{
	public class GameOverPanel : AbPanel
	{
		public override bool IsSolid => true;

		public void MainMenu()
		{
			GameplayManager.Instance.GoToMainMenu();
		}
	}
}