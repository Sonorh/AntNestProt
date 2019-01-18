using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PM.Antnest.Gameplay
{
	[CreateAssetMenu(fileName = "WarehouseConfig", menuName = "Config/BuildingConfig/WarehouseConfig", order = 0)]
	public class WarehouseConfig : BaseBuildingConfig
	{
		public WarehouseConfig()
		{
			this.Type = RoomTypes.Warehause;
		}
	}
}