using PM.UsefulThings.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PM.Antnest.Gameplay
{
	public class NestCell : AbCell
	{
		public new NestCellData Data { get; protected set; }

		public override void Reset(AbCellData data)
		{
			base.Reset(data);

			this.Data = data as NestCellData;

			UpdateInfo();
		}

		public override void UpdateInfo(AbCellData data = null)
		{
			if (data != null && data != this.Data)
			{
				return;
			}

			avatarColor.value = new Color32(56, 10, 10, 255);

			state.value = (GameplayManager.Instance.Player.Nest == this.Data) ? "Your".Localized() : "Enemy".Localized();
			value.value = "Nest".Localized();
		}
	}
}