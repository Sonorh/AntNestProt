using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PM.Antnest.Gameplay
{
	public class ConstructionData : AbNestRoomData
	{
		public RoomTypes TargetType;

		public ConstructionData() : base()
		{
			this.Type = RoomTypes.Construction;
		}

		public ConstructionData(RoomTypes targetType) : this()
		{
			this.TargetType = targetType;
		}
	}
}