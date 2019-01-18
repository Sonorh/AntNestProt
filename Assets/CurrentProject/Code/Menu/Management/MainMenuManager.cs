using PM.Antnest.General;
using PM.UsefulThings;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PM.Antnest.Menu
{
	public class MainMenuManager : SoftMonoSingleton<MainMenuManager>
	{
		protected override void Awake()
		{
			base.Awake();

			WindowManagerUT.Instance.CloseFrames();
			WindowManagerUT.Instance.ClosePanels();

			WindowManagerUT.Instance.OpenNewPanel<MainMenuPanel>();

			SoundManager.Instance?.PlayMusic(SoundManager.Instance?.MenuMusic);
		}


		public void StartTheGame()
		{
			SceneManager.LoadScene("Gameplay");
		}

		public void StartTutorial()
		{
			MainMenuHolderManager.Instance.Configs.CurrentGameConfig.Mimicry(MainMenuHolderManager.Instance.Configs.DefaultGameConfig);
			StartTheGame();
		}

		public void NewGame()
		{
			WindowManagerUT.Instance.OpenNewPanel<GameConfigPanel>(WindowCloseModes.CloseNonSolid);
		}

		public void LoadGame()
		{
			MainMenuHolderManager.Instance.Configs.CurrentGameConfig.Mimicry(MainMenuHolderManager.Instance.Configs.DefaultGameConfig);
			StartTheGame();
		}

		public void Settings()
		{
			WindowManagerUT.Instance.OpenNewPanel<SettingsPanel>(WindowCloseModes.CloseNonSolid);
		}

		public void Credits()
		{
			WindowManagerUT.Instance.OpenNewPanel<CreditsPanel>(WindowCloseModes.CloseNonSolid);
		}

		public void Quit()
		{
#if UNITY_EDITOR
			UnityEditor.EditorApplication.isPlaying = false;
#else
			Application.Quit();
#endif
		}
	}
}