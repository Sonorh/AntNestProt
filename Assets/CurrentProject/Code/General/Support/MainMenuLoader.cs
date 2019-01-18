using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PM.Antnest.General
{
	public class MainMenuLoader : MonoBehaviour
	{
		private void Start()
		{
			SceneManager.LoadScene("MainMenu");
		}
	}
}