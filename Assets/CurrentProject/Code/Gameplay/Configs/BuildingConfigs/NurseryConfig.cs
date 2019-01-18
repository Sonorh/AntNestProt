using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PM.Antnest.Gameplay
{
	[CreateAssetMenu(fileName = "NurseryConfig", menuName = "Config/BuildingConfig/NurseryConfig", order = 0)]
	public class NurseryConfig : BaseBuildingConfig
	{
		public NurseryConfig()
		{
			this.Type = RoomTypes.Nursery;
		}
	}
}