using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PM.Antnest.Gameplay
{
	[CreateAssetMenu(fileName = "ArmoryConfig", menuName = "Config/BuildingConfig/ArmoryConfig", order = 0)]
	public class ArmoryConfig : BaseBuildingConfig
	{
		public ArmoryConfig()
		{
			this.Type = RoomTypes.Armory;
		}
	}
}