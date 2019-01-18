using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UIBinding;
using PM.UsefulThings.Extensions;

namespace PM.Antnest.Gameplay
{
	public class NestAntCountData : BaseListElementData
	{
		private readonly StringProperty type = new StringProperty();
		private readonly IntProperty count = new IntProperty();
		private readonly IntProperty max = new IntProperty();

		public NestAntCountData(AntTypes type, int count)
		{
			this.type.value = type.ToString().Localized();
			this.count.value = count;
			this.max.value = GameplayManager.Instance.Player.Anter.AntMaxCapacity[type];
		}
	}
}