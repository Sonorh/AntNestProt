using PM.Antnest.General;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PM.Antnest.Gameplay
{
	//[CreateAssetMenu(fileName = "ConfigHolder", menuName = "ConfigHolder", order = 1)]
	public class ConfigHolder : ScriptableObject
	{
		public Balance Balance;
		public GameConfig GameSettings;
		public BugConfig[] BugConfigs;
		public AntConfig[] AntConfigs;
		public BaseBuildingConfig[] BuildingConfigs;
	}
}