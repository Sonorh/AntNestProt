using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PM.Antnest.Gameplay
{
	public class BuildingSpotData : AbNestRoomData
	{
		public BuildingSpotData() : base()
		{
			this.Type = RoomTypes.BuildingSpot;
		}
	}
}