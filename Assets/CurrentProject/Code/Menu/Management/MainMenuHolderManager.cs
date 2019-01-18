using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PM.UsefulThings;
using PM.Antnest.General;

namespace PM.Antnest.Menu
{
	public class MainMenuHolderManager : SoftMonoSingleton<MainMenuHolderManager>
	{
		public MenuPrefabHolder Prefabs;
		public MenuConfigHolder Configs;
		public MenuSpriteHolder Sprites;
	}
}
