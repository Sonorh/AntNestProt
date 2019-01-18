using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PM.Antnest.Gameplay
{
	public class EntranceData : AbNestRoomData
	{
		public EntranceData() : base()
		{
			this.Type = RoomTypes.Entrance;
		}
	}
}