using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PM.Antnest.Gameplay
{
    public abstract class BaseBuildingConfig : ScriptableObject
    {
		public RoomTypes Type { get; protected set; }
        public Image BaseImage;
		public int FruitCost;
		public int MeatCost;
	}
}
