using PM.Antnest.General;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PM.Antnest.Menu
{
	//[CreateAssetMenu(fileName = "MenuConfigHolder", menuName = "MenuConfigHolder", order = 1)]
	public class MenuConfigHolder : ScriptableObject
	{
		public GameConfig DefaultGameConfig;
		public GameConfig CurrentGameConfig;
		public GameConfigRangesConfig GCRangesConfig;
	}
}