using System.Collections;
using System.Collections.Generic;
using UIBinding;
using UnityEngine;

namespace PM.Antnest.Gameplay
{
	public class BuildingTypeData : BaseListElementData
	{
		private static Dictionary<RoomTypes, string> descriptions = new Dictionary<RoomTypes, string>()
		{
			{RoomTypes.Armory, "allows you to nurture 20 more warriors" },
			{RoomTypes.Nursery, "allows you to nurture 20 more workers" },
			{RoomTypes.Warehause, "allows you to store 100 more resources" },
		};
		
		public RoomTypes Type { get; private set; }

		private readonly StringProperty name = new StringProperty();
		private readonly IntProperty fruitCost = new IntProperty();
		private readonly IntProperty meatCost = new IntProperty();
		private readonly StringProperty description = new StringProperty();

		public BuildingTypeData(RoomTypes type, int fruitCost, int meatCost)
		{
			this.Type = type;
			this.name.value	 = type.ToString();
			this.fruitCost.value = fruitCost;
			this.meatCost.value = meatCost;
			this.description.value = descriptions[type];
		}
	}
}